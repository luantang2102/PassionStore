using PassionStore.Application.DTOs.Products;
using PassionStore.Application.Helpers.Params;
using PassionStore.Application.Interfaces;
using PassionStore.Application.Mappers;
using PassionStore.Application.Paginations;
using PassionStore.Core.Entities;
using PassionStore.Core.Exceptions;
using PassionStore.Core.Interfaces.IRepositories;
using PassionStore.Core.Models;
using PassionStore.Infrastructure.Extensions;
using PassionStore.Infrastructure.Externals;

namespace PassionStore.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly CloudinaryService cloudinaryService;
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(
            IProductRepository productRepository,
            ICartRepository cartRepository,
            IBrandRepository brandRepository,
            CloudinaryService cloudinaryService,
            IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _cartRepository = cartRepository;
            _brandRepository = brandRepository;
            this.cloudinaryService = cloudinaryService;
            _unitOfWork = unitOfWork;
        }

        public async Task<ProductResponse> GetProductByIdAsync(Guid productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                var attributes = new Dictionary<string, object>
                        {
                            { "productId", productId }
                        };
                throw new AppException(ErrorCode.PRODUCT_NOT_FOUND, attributes);
            }

            return product.MapModelToResponse();
        }

        public Task<PagedList<ProductResponse>> GetProductsAsync(ProductParams productParams)
        {
            var query = _productRepository.GetAllAsync()
                    .Sort(productParams.OrderBy)
                    .Search(productParams.SearchTerm)
                    .Filter(productParams.Categories, productParams.Ratings, productParams.MinPrice, productParams.MaxPrice, productParams.IsFeatured);

            var projectedQuery = query.Select(x => x.MapModelToResponse());

            return PaginationService.ToPagedList(
                projectedQuery,
                productParams.PageNumber,
                productParams.PageSize
            );
        }


        public async Task<PagedList<ProductResponse>> GetProductsByCategoryIdAsync(Guid categoryId, ProductParams productParams)
        {
            var query = _productRepository.GetByCategoryIdAsync(categoryId)
                .Sort(productParams.OrderBy)
                .Search(productParams.SearchTerm)
                .Filter(productParams.Categories, productParams.Ratings, productParams.MinPrice, productParams.MaxPrice, productParams.IsFeatured);

            var projectedQuery = query.Select(x => x.MapModelToResponse());

            return await PaginationService.ToPagedList(
                projectedQuery,
                productParams.PageNumber,
                productParams.PageSize
            );
        }

        public async Task<ProductResponse> CreateProductAsync(ProductRequest productRequest)
        {
            var product = new Product
            {
                Name = productRequest.Name,
                Description = productRequest.Description,
                InStock = productRequest.InStock,
                IsFeatured = productRequest.IsFeatured,
                ProductImages = [],
            };
            
            if(productRequest.BrandId != Guid.Empty)
            {
                var brand = await _brandRepository.GetByIdAsync(productRequest.BrandId);
                if (brand == null)
                {
                    var attributes = new Dictionary<string, object>
                        {
                            { "brandId", productRequest.BrandId }
                        };
                    throw new AppException(ErrorCode.BRAND_NOT_FOUND, attributes);
                }
                product.Brand = brand;
            }

            if (productRequest.CategoryIds.Count > 0)
            {
                var categories = await _productRepository.GetCategoriesByIdsAsync(productRequest.CategoryIds);
                if (categories.Count != productRequest.CategoryIds.Count)
                {
                    var attributes = new Dictionary<string, object>
                        {
                            { "categoryIds", productRequest.CategoryIds }
                        };
                    throw new AppException(ErrorCode.CATEGORY_NOT_FOUND, attributes);
                }
                product.Categories = categories;
            }

            if (productRequest.FormImages.Count > 0)
            {
                foreach (var image in productRequest.FormImages)
                {
                    var uploadResult = await cloudinaryService.AddImageAsync(image);
                    var productImage = new ProductImage
                    {
                        PublicId = uploadResult.PublicId,
                        ImageUrl = uploadResult.SecureUrl.AbsoluteUri,
                        Product = product
                    };
                    product.ProductImages.Add(productImage);
                }
            }

            var createdProduct = await _productRepository.CreateAsync(product);

            await _unitOfWork.CommitAsync();

            return createdProduct.MapModelToResponse();
        }

        public async Task<ProductResponse> UpdateProductAsync(Guid productId, ProductRequest productRequest)
        {
            var product = await _productRepository.GetWithImagesAsync(productId);
            if (product == null)
            {
                var attributes = new Dictionary<string, object>
                    {
                        { "productId", productId }
                    };
                throw new AppException(ErrorCode.PRODUCT_NOT_FOUND, attributes);
            }

            product.Name = productRequest.Name;
            product.Description = productRequest.Description;
            product.InStock = productRequest.InStock;
            product.IsFeatured = productRequest.IsFeatured;
            product.UpdatedDate = DateTime.UtcNow;

            if (productRequest.BrandId != Guid.Empty)
            {
                var brand = await _brandRepository.GetByIdAsync(productRequest.BrandId);
                if (brand == null)
                {
                    var attributes = new Dictionary<string, object>
                        {
                            { "brandId", productRequest.BrandId }
                        };
                    throw new AppException(ErrorCode.BRAND_NOT_FOUND, attributes);
                }
                product.Brand = brand;
            }

            var existingImages = product.ProductImages.ToList();
            var requestImageIds = productRequest.Images.Select(i => i.Id).ToList();

            foreach (var image in existingImages)
            {
                if (!requestImageIds.Contains(image.Id))
                {
                    await cloudinaryService.DeleteImageAsync(image.PublicId);
                    product.ProductImages.Remove(image);
                }
            }

            if (productRequest.CategoryIds.Count > 0)
            {
                var categories = await _productRepository.GetCategoriesByIdsAsync(productRequest.CategoryIds);
                if (categories.Count != productRequest.CategoryIds.Count)
                {
                    var attributes = new Dictionary<string, object>
                        {
                            { "categoryIds", productRequest.CategoryIds }
                        };
                    throw new AppException(ErrorCode.CATEGORY_NOT_FOUND, attributes);
                }

                var productWithCategories = await _productRepository.GetWithCategoriesAsync(productId);
                if (productWithCategories == null)
                {
                    var attributes = new Dictionary<string, object>
                        {
                            { "productId", productId }
                        };
                    throw new AppException(ErrorCode.PRODUCT_NOT_FOUND, attributes);
                }

                productWithCategories.Categories.Clear();
                foreach (var category in categories)
                {
                    productWithCategories.Categories.Add(category);
                }
                product.Categories = productWithCategories.Categories;
            }
            else
            {
                var productWithCategories = await _productRepository.GetWithCategoriesAsync(productId);
                if (productWithCategories == null)
                {
                    var attributes = new Dictionary<string, object>
                        {
                            { "productId", productId }
                        };
                    throw new AppException(ErrorCode.PRODUCT_NOT_FOUND, attributes);
                }
                productWithCategories.Categories.Clear();
                product.Categories = productWithCategories.Categories;
            }

            foreach (var existing in product.ProductImages)
            {
                var match = productRequest.Images.FirstOrDefault(i => i.Id == existing.Id);
                if (match != null)
                {
                    existing.IsMain = match.IsMain;
                }
            }

            if (productRequest.FormImages?.Count > 0)
            {
                foreach (var image in productRequest.FormImages)
                {
                    var uploadResult = await cloudinaryService.AddImageAsync(image);
                    var productImage = new ProductImage
                    {
                        PublicId = uploadResult.PublicId,
                        ImageUrl = uploadResult.SecureUrl.AbsoluteUri,
                        IsMain = false,
                        Product = product
                    };
                    product.ProductImages.Add(productImage);
                }
            }

            await _productRepository.UpdateAsync(product);

            await _unitOfWork.CommitAsync();

            return product.MapModelToResponse();
        }

        public async Task DeleteProductAsync(Guid productId)
        {
            var product = await _productRepository.GetWithImagesAsync(productId);
            if (product == null)
            {
                var attributes = new Dictionary<string, object>
                    {
                        { "productId", productId }
                    };
                throw new AppException(ErrorCode.PRODUCT_NOT_FOUND, attributes);
            }

            // Check if the product exists in any cart using the repository
            if (await _cartRepository.HasProductAsync(productId))
            {
                var attributes = new Dictionary<string, object>
                    {
                        { "productId", productId }
                    };
                throw new AppException(ErrorCode.PRODUCT_IN_CART, attributes);
            }

            // Delete associated images
            foreach (var image in product.ProductImages)
            {
                await cloudinaryService.DeleteImageAsync(image.PublicId);
            }

            // Delete the product
            await _productRepository.DeleteAsync(product);

            await _unitOfWork.CommitAsync();
        }

    }
}
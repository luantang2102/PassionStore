using PassionStore.Application.DTOs.Products;
using PassionStore.Application.Helpers.Params;
using PassionStore.Application.Interfaces;
using PassionStore.Application.Mappers;
using PassionStore.Application.Paginations;
using PassionStore.Core.Entities;
using PassionStore.Core.Entities.Constants;
using PassionStore.Core.Enums;
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
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly CloudinaryService _cloudinaryService;
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(
            IProductRepository productRepository,
            IProductVariantRepository productVariantRepository,
            IBrandRepository brandRepository,
            CloudinaryService cloudinaryService,
            IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _productVariantRepository = productVariantRepository;
            _brandRepository = brandRepository;
            _cloudinaryService = cloudinaryService;
            _unitOfWork = unitOfWork;
        }

        public async Task<ProductResponse> GetProductByIdAsync(Guid productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                var attributes = new Dictionary<string, object> { { "productId", productId } };
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

        public Task<PagedList<ProductResponse>> GetPopularProductsAsync(ProductParams productParams)
        {
            var query = _productRepository.GetPopularProductsAsync()
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
                IsNotHadVariants = productRequest.IsNotHadVariants
            };

            if (productRequest.BrandId != Guid.Empty)
            {
                var brand = await _brandRepository.GetByIdAsync(productRequest.BrandId);
                if (brand == null)
                {
                    var attributes = new Dictionary<string, object> { { "brandId", productRequest.BrandId } };
                    throw new AppException(ErrorCode.BRAND_NOT_FOUND, attributes);
                }
                product.Brand = brand;
            }

            if (productRequest.CategoryIds.Count > 0)
            {
                var categories = await _productRepository.GetCategoriesByIdsAsync(productRequest.CategoryIds);
                if (categories.Count != productRequest.CategoryIds.Count)
                {
                    var attributes = new Dictionary<string, object> { { "categoryIds", productRequest.CategoryIds } };
                    throw new AppException(ErrorCode.CATEGORY_NOT_FOUND, attributes);
                }
                product.Categories = categories;
            }

            if (productRequest.FormImages.Count > 0)
            {
                for (int i = 0; i < productRequest.FormImages.Count; i++)
                {
                    var image = productRequest.FormImages[i];
                    var uploadResult = await _cloudinaryService.AddImageAsync(image);
                    var productImage = new ProductImage
                    {
                        PublicId = uploadResult.PublicId,
                        ImageUrl = uploadResult.SecureUrl.AbsoluteUri,
                        Order = i,
                        Product = product
                    };
                    product.ProductImages.Add(productImage);
                }
            }

            if (productRequest.IsNotHadVariants)
            {
                var defaultVariant = new ProductVariant
                {
                    Price = productRequest.DefaultVariantPrice,
                    StockQuantity = productRequest.DefaultVariantStockQuantity,
                    Product = product,
                    ColorId = Constants.NoneColorId,
                    SizeId = Constants.NoneSizeId
                };
                product.ProductVariants.Add(defaultVariant);
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
                var attributes = new Dictionary<string, object> { { "productId", productId } };
                throw new AppException(ErrorCode.PRODUCT_NOT_FOUND, attributes);
            }

            product.Name = productRequest.Name;
            product.Description = productRequest.Description;
            product.InStock = productRequest.InStock;
            product.IsFeatured = productRequest.IsFeatured;
            product.UpdatedDate = DateTime.UtcNow;
            product.IsNotHadVariants = productRequest.IsNotHadVariants;

            if (productRequest.BrandId != Guid.Empty)
            {
                var brand = await _brandRepository.GetByIdAsync(productRequest.BrandId);
                if (brand == null)
                {
                    var attributes = new Dictionary<string, object> { { "brandId", productRequest.BrandId } };
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
                    await _cloudinaryService.DeleteImageAsync(image.PublicId);
                    product.ProductImages.Remove(image);
                }
            }

            for (int i = 0; i < productRequest.Images.Count; i++)
            {
                var match = product.ProductImages.FirstOrDefault(img => img.Id == productRequest.Images[i].Id);
                if (match != null)
                {
                    match.Order = i;
                }
            }

            int nextOrder = product.ProductImages.Any() ? product.ProductImages.Max(img => img.Order) + 1 : 0;
            if (productRequest.FormImages?.Count > 0)
            {
                foreach (var image in productRequest.FormImages)
                {
                    var uploadResult = await _cloudinaryService.AddImageAsync(image);
                    var productImage = new ProductImage
                    {
                        PublicId = uploadResult.PublicId,
                        ImageUrl = uploadResult.SecureUrl.AbsoluteUri,
                        Order = nextOrder++,
                        Product = product
                    };
                    product.ProductImages.Add(productImage);
                }
            }

            if (productRequest.CategoryIds.Count > 0)
            {
                var categories = await _productRepository.GetCategoriesByIdsAsync(productRequest.CategoryIds);
                if (categories.Count != productRequest.CategoryIds.Count)
                {
                    var attributes = new Dictionary<string, object> { { "categoryIds", productRequest.CategoryIds } };
                    throw new AppException(ErrorCode.CATEGORY_NOT_FOUND, attributes);
                }

                var productWithCategories = await _productRepository.GetWithCategoriesAsync(productId);
                if (productWithCategories == null)
                {
                    var attributes = new Dictionary<string, object> { { "productId", productId } };
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
                    var attributes = new Dictionary<string, object> { { "productId", productId } };
                    throw new AppException(ErrorCode.PRODUCT_NOT_FOUND, attributes);
                }
                productWithCategories.Categories.Clear();
                product.Categories = productWithCategories.Categories;
            }

            var variants = await _productVariantRepository.GetByProductIdAsync(productId);
            if (productRequest.IsNotHadVariants)
            {
                var defaultVariant = variants.FirstOrDefault(v => v.ColorId == Constants.NoneColorId && v.SizeId == Constants.NoneSizeId);
                if (defaultVariant == null)
                {
                    defaultVariant = new ProductVariant
                    {
                        Price = productRequest.DefaultVariantPrice,
                        StockQuantity = productRequest.DefaultVariantStockQuantity,
                        ProductId = productId,
                        ColorId = Constants.NoneColorId,
                        SizeId = Constants.NoneSizeId
                    };
                    await _productVariantRepository.CreateAsync(defaultVariant);
                }
                else
                {
                    defaultVariant.Price = productRequest.DefaultVariantPrice;
                    defaultVariant.StockQuantity = productRequest.DefaultVariantStockQuantity;
                    defaultVariant.UpdatedDate = DateTime.UtcNow;
                    await _productVariantRepository.UpdateAsync(defaultVariant);
                }

                foreach (var variant in variants.Where(v => v.ColorId != Constants.NoneColorId || v.SizeId != Constants.NoneSizeId))
                {
                    if (await _productVariantRepository.HasProductVariantAsync(variant.Id))
                    {
                        var attributes = new Dictionary<string, object> { { "productVariantId", variant.Id } };
                        throw new AppException(ErrorCode.PRODUCT_VARIANT_IN_CART, attributes);
                    }
                    await _productVariantRepository.DeleteAsync(variant);
                }
            }
            else
            {
                foreach (var variant in variants)
                {
                    if (variant.ColorId == Constants.NoneColorId && variant.SizeId == Constants.NoneSizeId)
                    {
                        await _productVariantRepository.DeleteAsync(variant);
                    }
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
                var attributes = new Dictionary<string, object> { { "productId", productId } };
                throw new AppException(ErrorCode.PRODUCT_NOT_FOUND, attributes);
            }

            var variants = await _productVariantRepository.GetByProductIdAsync(productId);
            foreach (var variant in variants)
            {
                if (await _productVariantRepository.HasProductVariantAsync(variant.Id))
                {
                    var attributes = new Dictionary<string, object> { { "productVariantId", variant.Id } };
                    throw new AppException(ErrorCode.PRODUCT_VARIANT_IN_CART, attributes);
                }
            }

            foreach (var image in product.ProductImages)
            {
                await _cloudinaryService.DeleteImageAsync(image.PublicId);
            }

            await _productRepository.DeleteAsync(product);
            await _unitOfWork.CommitAsync();
        }
    }
}
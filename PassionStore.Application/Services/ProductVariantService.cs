using PassionStore.Application.DTOs.Products;
using PassionStore.Application.DTOs.ProductVariants;
using PassionStore.Application.Helpers.Params;
using PassionStore.Application.Interfaces;
using PassionStore.Application.Mappers;
using PassionStore.Application.Paginations;
using PassionStore.Core.Exceptions;
using PassionStore.Core.Interfaces.IRepositories;
using PassionStore.Core.Models;
using PassionStore.Infrastructure.Extensions;
using PassionStore.Infrastructure.Externals;

namespace PassionStore.Application.Services
{
    public class ProductVariantService : IProductVariantService
    {
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly ICartRepository _cartRepository;
        private readonly CloudinaryService _cloudinaryService;
        private readonly IUnitOfWork _unitOfWork;

        public ProductVariantService(
            IProductVariantRepository productVariantRepository,
            ICartRepository cartRepository,
            CloudinaryService cloudinaryService,
            IUnitOfWork unitOfWork)
        {
            _productVariantRepository = productVariantRepository;
            _cartRepository = cartRepository;
            _cloudinaryService = cloudinaryService;
            _unitOfWork = unitOfWork;
        }

        public async Task<ProductVariantResponse> GetProductVariantByIdAsync(Guid productVariantId)
        {
            var variant = await _productVariantRepository.GetByIdAsync(productVariantId);
            if (variant == null)
            {
                var attributes = new Dictionary<string, object> { { "productVariantId", productVariantId } };
                throw new AppException(ErrorCode.PRODUCT_VARIANT_NOT_FOUND, attributes);
            }
            return variant.MapModelToResponse();
        }

        public async Task<PagedList<ProductVariantResponse>> GetProductVariantsAsync(ProductVariantParams productVariantParams)
        {
            var query = _productVariantRepository.GetAllAsync()
                .Sort(productVariantParams.OrderBy)
                .Filter(productVariantParams.ProductId, productVariantParams.ColorId, productVariantParams.SizeId, productVariantParams.MinPrice, productVariantParams.MaxPrice);

            var projectedQuery = query.Select(x => x.MapModelToResponse());

            return await PaginationService.ToPagedList(projectedQuery, productVariantParams.PageNumber, productVariantParams.PageSize);
        }

        public async Task<ProductVariantResponse> CreateProductVariantAsync(ProductVariantRequest productVariantRequest)
        {
            var product = await _productVariantRepository.GetProductByIdAsync(productVariantRequest.ProductId);
            if (product == null)
            {
                var attributes = new Dictionary<string, object> { { "productId", productVariantRequest.ProductId } };
                throw new AppException(ErrorCode.PRODUCT_NOT_FOUND, attributes);
            }

            var color = await _productVariantRepository.GetColorByIdAsync(productVariantRequest.ColorId);
            if (color == null)
            {
                var attributes = new Dictionary<string, object> { { "colorId", productVariantRequest.ColorId } };
                throw new AppException(ErrorCode.COLOR_NOT_FOUND, attributes);
            }

            var size = await _productVariantRepository.GetSizeByIdAsync(productVariantRequest.SizeId);
            if (size == null)
            {
                var attributes = new Dictionary<string, object> { { "sizeId", productVariantRequest.SizeId } };
                throw new AppException(ErrorCode.SIZE_NOT_FOUND, attributes);
            }

            var variant = new ProductVariant
            {
                Price = productVariantRequest.Price,
                StockQuantity = productVariantRequest.StockQuantity,
                ProductId = productVariantRequest.ProductId,
                ColorId = productVariantRequest.ColorId,
                SizeId = productVariantRequest.SizeId,
                ProductVariantImages = []
            };

            if (productVariantRequest.FormImages?.Count > 0)
            {
                foreach (var image in productVariantRequest.FormImages)
                {
                    var uploadResult = await _cloudinaryService.AddImageAsync(image);
                    var variantImage = new ProductVariantImage
                    {
                        PublicId = uploadResult.PublicId,
                        ImageUrl = uploadResult.SecureUrl.AbsoluteUri,
                        IsMain = false,
                        ProductVariant = variant
                    };
                    variant.ProductVariantImages.Add(variantImage);
                }
            }

            var createdVariant = await _productVariantRepository.CreateAsync(variant);
            await _unitOfWork.CommitAsync();
            return createdVariant.MapModelToResponse();
        }

        public async Task<ProductVariantResponse> UpdateProductVariantAsync(Guid productVariantId, ProductVariantRequest productVariantRequest)
        {
            var variant = await _productVariantRepository.GetWithImagesAsync(productVariantId);
            if (variant == null)
            {
                var attributes = new Dictionary<string, object> { { "productVariantId", productVariantId } };
                throw new AppException(ErrorCode.PRODUCT_VARIANT_NOT_FOUND, attributes);
            }

            var product = await _productVariantRepository.GetProductByIdAsync(productVariantRequest.ProductId);
            if (product == null)
            {
                var attributes = new Dictionary<string, object> { { "productId", productVariantRequest.ProductId } };
                throw new AppException(ErrorCode.PRODUCT_NOT_FOUND, attributes);
            }

            var color = await _productVariantRepository.GetColorByIdAsync(productVariantRequest.ColorId);
            if (color == null)
            {
                var attributes = new Dictionary<string, object> { { "colorId", productVariantRequest.ColorId } };
                throw new AppException(ErrorCode.COLOR_NOT_FOUND, attributes);
            }

            var size = await _productVariantRepository.GetSizeByIdAsync(productVariantRequest.SizeId);
            if (size == null)
            {
                var attributes = new Dictionary<string, object> { { "sizeId", productVariantRequest.SizeId } };
                throw new AppException(ErrorCode.SIZE_NOT_FOUND, attributes);
            }

            variant.Price = productVariantRequest.Price;
            variant.StockQuantity = productVariantRequest.StockQuantity;
            variant.ProductId = productVariantRequest.ProductId;
            variant.ColorId = productVariantRequest.ColorId;
            variant.SizeId = productVariantRequest.SizeId;
            variant.UpdatedDate = DateTime.UtcNow;

            var existingImages = variant.ProductVariantImages.ToList();
            var requestImageIds = productVariantRequest.Images?.Select(i => i.Id).ToList() ?? new List<Guid>();

            foreach (var image in existingImages)
            {
                if (!requestImageIds.Contains(image.Id))
                {
                    await _cloudinaryService.DeleteImageAsync(image.PublicId);
                    variant.ProductVariantImages.Remove(image);
                }
            }

            foreach (var existing in variant.ProductVariantImages)
            {
                var match = productVariantRequest.Images?.FirstOrDefault(i => i.Id == existing.Id);
                if (match != null)
                {
                    existing.IsMain = match.IsMain;
                }
            }

            if (productVariantRequest.FormImages?.Count > 0)
            {
                foreach (var image in productVariantRequest.FormImages)
                {
                    var uploadResult = await _cloudinaryService.AddImageAsync(image);
                    var variantImage = new ProductVariantImage
                    {
                        PublicId = uploadResult.PublicId,
                        ImageUrl = uploadResult.SecureUrl.AbsoluteUri,
                        IsMain = false,
                        ProductVariant = variant
                    };
                    variant.ProductVariantImages.Add(variantImage);
                }
            }

            await _productVariantRepository.UpdateAsync(variant);
            await _unitOfWork.CommitAsync();
            return variant.MapModelToResponse();
        }

        public async Task DeleteProductVariantAsync(Guid productVariantId)
        {
            var variant = await _productVariantRepository.GetWithImagesAsync(productVariantId);
            if (variant == null)
            {
                var attributes = new Dictionary<string, object> { { "productVariantId", productVariantId } };
                throw new AppException(ErrorCode.PRODUCT_VARIANT_NOT_FOUND, attributes);
            }

            if (await _cartRepository.HasProductVariantAsync(productVariantId))
            {
                var attributes = new Dictionary<string, object> { { "productVariantId", productVariantId } };
                throw new AppException(ErrorCode.PRODUCT_VARIANT_IN_CART, attributes);
            }

            foreach (var image in variant.ProductVariantImages)
            {
                await _cloudinaryService.DeleteImageAsync(image.PublicId);
            }

            await _productVariantRepository.DeleteAsync(variant);
            await _unitOfWork.CommitAsync();
        }
    }
}
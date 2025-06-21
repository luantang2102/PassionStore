using PassionStore.Application.DTOs.ProductVariants;
using PassionStore.Core.Exceptions;
using PassionStore.Core.Interfaces.IRepositories;
using PassionStore.Core.Models;
using PassionStore.Infrastructure.Externals;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using PassionStore.Application.Helpers.Params;
using PassionStore.Application.Mappers;
using PassionStore.Application.Paginations;
using PassionStore.Infrastructure.Extensions;
using PassionStore.Application.Interfaces;
using PassionStore.Core.Entities.Constants;

namespace PassionStore.Application.Services
{
    public class ProductVariantService : IProductVariantService
    {
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly CloudinaryService _cloudinaryService;
        private readonly IUnitOfWork _unitOfWork;

        public ProductVariantService(
            IProductVariantRepository productVariantRepository,
            CloudinaryService cloudinaryService,
            IUnitOfWork unitOfWork)
        {
            _productVariantRepository = productVariantRepository;
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

        public async Task<ProductVariantResponse> CreateAsync(ProductVariantRequest request)
        {
            if (await _productVariantRepository.ExistsAsync(request.ProductId, request.ColorId, request.SizeId))
            {
                var attributes = new Dictionary<string, object>
                {
                    { "productId", request.ProductId },
                    { "colorId", request.ColorId },
                    { "sizeId", request.SizeId }
                };
                throw new AppException(ErrorCode.PRODUCT_VARIANT_ALREADY_EXISTS, attributes);
            }

            var product = await _productVariantRepository.GetProductByIdAsync(request.ProductId);
            if (product == null)
            {
                var attributes = new Dictionary<string, object> { { "productId", request.ProductId } };
                throw new AppException(ErrorCode.PRODUCT_NOT_FOUND, attributes);
            }

            var color = await _productVariantRepository.GetColorByIdAsync(request.ColorId);
            if (color == null)
            {
                var attributes = new Dictionary<string, object> { { "colorId", request.ColorId } };
                throw new AppException(ErrorCode.COLOR_NOT_FOUND, attributes);
            }

            var size = await _productVariantRepository.GetSizeByIdAsync(request.SizeId);
            if (size == null)
            {
                var attributes = new Dictionary<string, object> { { "sizeId", request.SizeId } };
                throw new AppException(ErrorCode.SIZE_NOT_FOUND, attributes);
            }

            var variant = new ProductVariant
            {
                Price = request.Price,
                StockQuantity = request.StockQuantity,
                ProductId = request.ProductId,
                ColorId = request.ColorId,
                SizeId = request.SizeId,
            };

            var createdVariant = await _productVariantRepository.CreateAsync(variant);
            await _unitOfWork.CommitAsync();
            return createdVariant.MapModelToResponse();
        }

        public async Task<ProductVariantResponse> UpdateAsync(Guid productVariantId, ProductVariantRequest request)
        {
            var variant = await _productVariantRepository.GetByIdAsync(productVariantId);
            if (variant == null)
            {
                var attributes = new Dictionary<string, object> { { "productVariantId", productVariantId } };
                throw new AppException(ErrorCode.PRODUCT_VARIANT_NOT_FOUND, attributes);
            }

            if (await _productVariantRepository.ExistsAsync(request.ProductId, request.ColorId, request.SizeId) &&
                (variant.ProductId != request.ProductId || variant.ColorId != request.ColorId || variant.SizeId != request.SizeId))
            {
                var attributes = new Dictionary<string, object>
                {
                    { "productId", request.ProductId },
                    { "colorId", request.ColorId },
                    { "sizeId", request.SizeId }
                };
                throw new AppException(ErrorCode.PRODUCT_VARIANT_ALREADY_EXISTS, attributes);
            }

            var product = await _productVariantRepository.GetProductByIdAsync(request.ProductId);
            if (product == null)
            {
                var attributes = new Dictionary<string, object> { { "productId", request.ProductId } };
                throw new AppException(ErrorCode.PRODUCT_NOT_FOUND, attributes);
            }

            var color = await _productVariantRepository.GetColorByIdAsync(request.ColorId);
            if (color == null)
            {
                var attributes = new Dictionary<string, object> { { "colorId", request.ColorId } };
                throw new AppException(ErrorCode.COLOR_NOT_FOUND, attributes);
            }

            var size = await _productVariantRepository.GetSizeByIdAsync(request.SizeId);
            if (size == null)
            {
                var attributes = new Dictionary<string, object> { { "sizeId", request.SizeId } };
                throw new AppException(ErrorCode.SIZE_NOT_FOUND, attributes);
            }

            variant.Price = request.Price;
            variant.StockQuantity = request.StockQuantity;
            variant.ProductId = request.ProductId;
            variant.ColorId = request.ColorId;
            variant.SizeId = request.SizeId;
            variant.UpdatedDate = DateTime.UtcNow;

            await _productVariantRepository.UpdateAsync(variant);
            await _unitOfWork.CommitAsync();
            return variant.MapModelToResponse();
        }

        public async Task DeleteAsync(Guid productVariantId)
        {
            var variant = await _productVariantRepository.GetByIdAsync(productVariantId);
            if (variant == null)
            {
                var attributes = new Dictionary<string, object> { { "productVariantId", productVariantId } };
                throw new AppException(ErrorCode.PRODUCT_VARIANT_NOT_FOUND, attributes);
            }

            var variants = await _productVariantRepository.GetByProductIdAsync(variant.ProductId);
            if (variants.Count == 1 && variant.ColorId == Constants.NoneColorId && variant.SizeId == Constants.NoneSizeId)
            {
                var attributes = new Dictionary<string, object> { { "productVariantId", productVariantId } };
                throw new AppException(ErrorCode.CANNOT_DELETE_DEFAULT_VARIANT, attributes);
            }

            if (await _productVariantRepository.HasProductVariantAsync(productVariantId))
            {
                var attributes = new Dictionary<string, object> { { "productVariantId", productVariantId } };
                throw new AppException(ErrorCode.PRODUCT_VARIANT_IN_CART, attributes);
            }

            await _productVariantRepository.DeleteAsync(variant);
            await _unitOfWork.CommitAsync();
        }
    }
}
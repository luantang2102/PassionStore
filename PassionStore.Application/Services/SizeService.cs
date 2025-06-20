using Microsoft.EntityFrameworkCore;
using PassionStore.Application.DTOs.Sizes;
using PassionStore.Application.Helpers.Params;
using PassionStore.Application.Interfaces;
using PassionStore.Application.Mappers;
using PassionStore.Application.Paginations;
using PassionStore.Core.Exceptions;
using PassionStore.Core.Interfaces.IRepositories;
using PassionStore.Core.Models;
using PassionStore.Infrastructure.Extensions;

namespace PassionStore.Application.Services
{
    public class SizeService : ISizeService
    {
        private readonly ISizeRepository _sizeRepository;
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SizeService(
            ISizeRepository sizeRepository,
            IProductVariantRepository productVariantRepository,
            IUnitOfWork unitOfWork)
        {
            _sizeRepository = sizeRepository;
            _productVariantRepository = productVariantRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<SizeResponse> GetSizeByIdAsync(Guid sizeId)
        {
            var size = await _sizeRepository.GetByIdAsync(sizeId);
            if (size == null)
            {
                var attributes = new Dictionary<string, object> { { "sizeId", sizeId } };
                throw new AppException(ErrorCode.SIZE_NOT_FOUND, attributes);
            }
            return size.MapModelToResponse();
        }

        public async Task<List<SizeResponse>> GetSizesAsync()
        {
            var sizes = _sizeRepository.GetAllAsync();
            return await sizes.Select(x => x.MapModelToResponse()).ToListAsync();
        }

        public async Task<PagedList<SizeResponse>> GetSizesAsync(SizeParams sizeParams)
        {
            var query = _sizeRepository.GetAllAsync()
                .Sort(sizeParams.OrderBy)
                .Search(sizeParams.SearchTerm);

            var projectedQuery = query.Select(x => x.MapModelToResponse());

            return await PaginationService.ToPagedList(projectedQuery, sizeParams.PageNumber, sizeParams.PageSize);
        }

        public async Task<SizeResponse> CreateSizeAsync(SizeRequest sizeRequest)
        {
            var size = new Size
            {
                Name = sizeRequest.Name
            };

            var createdSize = await _sizeRepository.CreateAsync(size);
            await _unitOfWork.CommitAsync();
            return createdSize.MapModelToResponse();
        }

        public async Task<SizeResponse> UpdateSizeAsync(Guid sizeId, SizeRequest sizeRequest)
        {
            var size = await _sizeRepository.GetByIdAsync(sizeId);
            if (size == null)
            {
                var attributes = new Dictionary<string, object> { { "sizeId", sizeId } };
                throw new AppException(ErrorCode.SIZE_NOT_FOUND, attributes);
            }

            size.Name = sizeRequest.Name;
            size.UpdatedDate = DateTime.UtcNow;

            await _sizeRepository.UpdateAsync(size);
            await _unitOfWork.CommitAsync();
            return size.MapModelToResponse();
        }

        public async Task DeleteSizeAsync(Guid sizeId)
        {
            var size = await _sizeRepository.GetByIdAsync(sizeId);
            if (size == null)
            {
                var attributes = new Dictionary<string, object> { { "sizeId", sizeId } };
                throw new AppException(ErrorCode.SIZE_NOT_FOUND, attributes);
            }

            if (await _productVariantRepository.HasSizeAsync(sizeId))
            {
                var attributes = new Dictionary<string, object> { { "sizeId", sizeId } };
                throw new AppException(ErrorCode.SIZE_IN_USE, attributes);
            }

            await _sizeRepository.DeleteAsync(size);
            await _unitOfWork.CommitAsync();
        }
    }
}
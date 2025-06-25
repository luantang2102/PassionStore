using Microsoft.EntityFrameworkCore;
using PassionStore.Application.DTOs.Brands;
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
    public class BrandService : IBrandService
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public BrandService(
            IBrandRepository brandRepository,
            IProductRepository productRepository,
            IUnitOfWork unitOfWork)
        {
            _brandRepository = brandRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<BrandResponse> GetBrandByIdAsync(Guid brandId)
        {
            var brand = await _brandRepository.GetByIdAsync(brandId);
            if (brand == null)
            {
                var attributes = new Dictionary<string, object> { { "brandId", brandId } };
                throw new AppException(ErrorCode.BRAND_NOT_FOUND, attributes);
            }
            return brand.MapModelToResponse();
        }

        public async Task<List<BrandResponse>> GetBrandsAsync()
        {
            var brands = _brandRepository.GetAllAsync();
            return await brands.Select(x => x.MapModelToResponse()).ToListAsync();
        }

        public async Task<PagedList<BrandResponse>> GetBrandsAsync(BrandParams brandParams)
        {
            var query = _brandRepository.GetAllAsync()
                .Sort(brandParams.OrderBy)
                .Search(brandParams.SearchTerm);

            var projectedQuery = query.Select(x => x.MapModelToResponse());

            return await PaginationService.ToPagedList(projectedQuery, brandParams.PageNumber, brandParams.PageSize);
        }

        public async Task<BrandResponse> CreateBrandAsync(BrandRequest brandRequest)
        {
            var brand = new Brand
            {
                Name = brandRequest.Name,
                Description = brandRequest.Description
            };

            var createdBrand = await _brandRepository.CreateAsync(brand);
            await _unitOfWork.CommitAsync();
            return createdBrand.MapModelToResponse();
        }

        public async Task<BrandResponse> UpdateBrandAsync(Guid brandId, BrandRequest brandRequest)
        {
            var brand = await _brandRepository.GetByIdAsync(brandId);
            if (brand == null)
            {
                var attributes = new Dictionary<string, object> { { "brandId", brandId } };
                throw new AppException(ErrorCode.BRAND_NOT_FOUND, attributes);
            }

            brand.Name = brandRequest.Name;
            brand.Description = brandRequest.Description;
            brand.UpdatedDate = DateTime.UtcNow;

            await _brandRepository.UpdateAsync(brand);
            await _unitOfWork.CommitAsync();
            return brand.MapModelToResponse();
        }

        public async Task DeleteBrandAsync(Guid brandId)
        {
            var brand = await _brandRepository.GetByIdAsync(brandId);
            if (brand == null)
            {
                var attributes = new Dictionary<string, object> { { "brandId", brandId } };
                throw new AppException(ErrorCode.BRAND_NOT_FOUND, attributes);
            }

            if (await _productRepository.HasBrandAsync(brandId))
            {
                var attributes = new Dictionary<string, object> { { "brandId", brandId } };
                throw new AppException(ErrorCode.BRAND_IN_USE, attributes);
            }

            await _brandRepository.DeleteAsync(brand);
            await _unitOfWork.CommitAsync();
        }
    }
}
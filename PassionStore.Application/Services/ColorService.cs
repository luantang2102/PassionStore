using Microsoft.EntityFrameworkCore;
using PassionStore.Application.DTOs.Colors;
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
    public class ColorService : IColorService
    {
        private readonly IColorRepository _colorRepository;
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ColorService(
            IColorRepository colorRepository,
            IProductVariantRepository productVariantRepository,
            IUnitOfWork unitOfWork)
        {
            _colorRepository = colorRepository;
            _productVariantRepository = productVariantRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ColorResponse> GetColorByIdAsync(Guid colorId)
        {
            var color = await _colorRepository.GetByIdAsync(colorId);
            if (color == null)
            {
                var attributes = new Dictionary<string, object> { { "colorId", colorId } };
                throw new AppException(ErrorCode.COLOR_NOT_FOUND, attributes);
            }
            return color.MapModelToResponse();
        }

        public async Task<List<ColorResponse>> GetColorsAsync()
        {
            var colors = _colorRepository.GetAllAsync();
            return await colors.Select(x => x.MapModelToResponse()).ToListAsync();
        }

        public async Task<PagedList<ColorResponse>> GetColorsAsync(ColorParams colorParams)
        {
            var query = _colorRepository.GetAllAsync()
                .Sort(colorParams.OrderBy)
                .Search(colorParams.SearchTerm);

            var projectedQuery = query.Select(x => x.MapModelToResponse());

            return await PaginationService.ToPagedList(projectedQuery, colorParams.PageNumber, colorParams.PageSize);
        }

        public async Task<ColorResponse> CreateColorAsync(ColorRequest colorRequest)
        {
            var color = new Color
            {
                Name = colorRequest.Name,
                HexCode = colorRequest.HexCode
            };

            var createdColor = await _colorRepository.CreateAsync(color);
            await _unitOfWork.CommitAsync();
            return createdColor.MapModelToResponse();
        }

        public async Task<ColorResponse> UpdateColorAsync(Guid colorId, ColorRequest colorRequest)
        {
            var color = await _colorRepository.GetByIdAsync(colorId);
            if (color == null)
            {
                var attributes = new Dictionary<string, object> { { "colorId", colorId } };
                throw new AppException(ErrorCode.COLOR_NOT_FOUND, attributes);
            }

            color.Name = colorRequest.Name;
            color.HexCode = colorRequest.HexCode;
            color.UpdatedDate = DateTime.UtcNow;

            await _colorRepository.UpdateAsync(color);
            await _unitOfWork.CommitAsync();
            return color.MapModelToResponse();
        }

        public async Task DeleteColorAsync(Guid colorId)
        {
            var color = await _colorRepository.GetByIdAsync(colorId);
            if (color == null)
            {
                var attributes = new Dictionary<string, object> { { "colorId", colorId } };
                throw new AppException(ErrorCode.COLOR_NOT_FOUND, attributes);
            }

            if (await _productVariantRepository.HasColorAsync(colorId))
            {
                var attributes = new Dictionary<string, object> { { "colorId", colorId } };
                throw new AppException(ErrorCode.COLOR_IN_USE, attributes);
            }

            await _colorRepository.DeleteAsync(color);
            await _unitOfWork.CommitAsync();
        }
    }
}
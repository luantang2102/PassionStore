﻿using Microsoft.AspNetCore.Mvc;
using PassionStore.Web.Models.Views;

namespace PassionStore.Web.Controllers.Components
{
    public class QuickViewModalViewComponent : ViewComponent
    {
        private readonly IProductService _productService;

        public QuickViewModalViewComponent(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid? productId, CancellationToken cancellationToken = default)
        {
            if (!productId.HasValue || productId == Guid.Empty)
            {
                return View(new ProductView());
            }
            var product = await _productService.GetProductByIdAsync(productId.Value, cancellationToken);
            return View(product ?? new ProductView());
        }
    }
}
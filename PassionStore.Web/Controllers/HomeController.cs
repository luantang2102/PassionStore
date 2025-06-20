using Microsoft.AspNetCore.Mvc;
using PassionStore.Web.Models;
using PassionStore.Web.Models.Views;
using PassionStore.Web.Services;
using System.Diagnostics;

namespace PassionStore.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;

    public HomeController(ILogger<HomeController> logger, IProductService productService, ICategoryService categoryService)
    {
        _logger = logger;
        _productService = productService;
        _categoryService = categoryService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var products = await _productService.GetFeaturedProductsAsync(
            new PaginationParams
            {
                PageNumber = 1,
                PageSize = 8
            },
            cancellationToken
        );

        var categories = await _categoryService.GetCategoriesTreeAsync(cancellationToken);


        return View(new HomeView
        {
            Products = products,
            Categories = categories
        });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

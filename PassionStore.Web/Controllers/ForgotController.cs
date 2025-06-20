﻿using Microsoft.AspNetCore.Mvc;

namespace PassionStore.Web.Controllers
{
    public class ForgotController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ForgotController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient("NashApp.Api");
            _configuration = configuration;
        }

        // GET: /Forgot
        [HttpGet]
        public IActionResult Index()
        {
            return View(new ForgotPasswordModel());
        }

        // POST: /Forgot
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([FromForm] ForgotPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var formData = new MultipartFormDataContent
                {
                    { new StringContent(model.Email), "email" }
                };

                var response = await _httpClient.PostAsync("/api/Auth/forgot-password", formData);
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Liên kết đặt lại mật khẩu đã được gửi đến email của bạn.";
                    return RedirectToAction("Index", "Login");
                }

                ModelState.AddModelError("", result?.Message ?? "Không thể gửi liên kết đặt lại. Vui lòng thử lại.");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Không thể kết nối đến máy chủ. Vui lòng thử lại sau.");
                return View(model);
            }
        }
    }

    public class ForgotPasswordModel
    {
        public string Email { get; set; }
    }
}
﻿using Microsoft.AspNetCore.Mvc;

namespace PassionStore.Web.Controllers
{
    public class LogoutController : Controller
    {
        private readonly HttpClient _httpClient;

        public LogoutController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("AuthApi");
        }

        // POST: /Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index()
        {
            try
            {
                await _httpClient.PostAsync("/api/Auth/logout", null);
            }
            catch
            {
                // Log error but proceed with client-side cleanup
            }

            Response.Cookies.Delete("nash_session");
            Response.Cookies.Delete("auth_jwt");
            Response.Cookies.Delete("refresh");
            Response.Cookies.Delete("csrf");
            return RedirectToAction("Index", "Home");
        }
    }
}
using Ims.Models;
using Ims.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ImsWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class SupplierController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl = "https://localhost:7214/api/Suppliers"; // API Base URL
        public SupplierController()
        {
            _httpClient = new HttpClient();
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ViewSuppliers()
        {
            var response = await _httpClient.GetAsync(_apiBaseUrl);
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var suppliers = JsonConvert.DeserializeObject<List<Supplier>>(jsonData);
                return View(suppliers);
            }

            TempData["ErrorMessage"] = "Error retrieving book list.";
            return View(new List<Supplier>());
        }
    }
}

using Ims.DataAccess.Data;
using Ims.Models;
using Ims.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ImsWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class BuyProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl = "https://localhost:7214/api/Suppliers";

        public BuyProductController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _httpClient = new HttpClient();
        }

        // GET: Display Available Suppliers
        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync(_apiBaseUrl);
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var suppliers = JsonConvert.DeserializeObject<List<Supplier>>(jsonData);
                return View(suppliers);
            }

            TempData["ErrorMessage"] = "Error retrieving supplier list.";
            return View(new List<Supplier>());
        }

        // GET: Display Products from Selected Supplier
        public async Task<IActionResult> SelectProduct(int supplierId)
        {
            var products = await _context.Products
                .Where(p => p.SupplierId == supplierId)
                .ToListAsync();

            ViewBag.SupplierId = supplierId;
            return View(products);
        }

        // GET: Show form to enter quantity and calculate credit availability
        public async Task<IActionResult> OrderForm(int productId, int supplierId)
        {
            var product = await _context.Products.FindAsync(productId);
            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id; // Assuming this method gets logged-in user ID
            var credit = await _context.CreditManagements
                .FirstOrDefaultAsync(c => c.UserId == userId && c.SupplierId == supplierId);

            if (credit == null)
            {
                ViewBag.ErrorMessage = "You don't have a credit arrangement with this supplier.";
                return View("ErrorPage");
            }

            ViewBag.CreditLimit = credit.CreditLimit;
            ViewBag.OutstandingBalance = credit.OutstandingBalance;
            ViewBag.MaxPurchaseAmount = credit.CreditLimit - credit.OutstandingBalance;

            return View(product);
        }

        // POST: Validate Quantity and Credit Limit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessOrder(int productId, int supplierId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);
            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id;
            var credit = await _context.CreditManagements
                .FirstOrDefaultAsync(c => c.UserId == userId && c.SupplierId == supplierId);

            if (credit == null)
            {
                TempData["Error"] = "No credit arrangement found.";
                return RedirectToAction("Index");
            }

            decimal totalAmount = product.UnitPrice * quantity;
            decimal availableCredit = credit.CreditLimit - credit.OutstandingBalance;

            if (totalAmount > availableCredit)
            {
                int maxQuantity = (int)(availableCredit / product.UnitPrice);
                TempData["Error"] = $"Not enough credit available. Max quantity you can buy: {maxQuantity}.";
                return RedirectToAction("OrderForm", new { productId, supplierId });
            }


            product.QuantityStock += quantity;
            credit.OutstandingBalance += totalAmount;
            credit.LastUpdated = DateTime.Now;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Order placed successfully!";
            return RedirectToAction("Index");
        }
    }
}

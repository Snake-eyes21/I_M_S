using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ims.DataAccess.Data;
using Ims.Models;
using Microsoft.AspNetCore.Identity;
using Ims.Models.ViewModels;
using Ims.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.CodeAnalysis;

namespace ImsWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = SD.Role_Customer)]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _usermanager;

        public OrdersController(ApplicationDbContext context,UserManager<IdentityUser> userManager)
        {
            _usermanager = userManager;
            _context = context;
        }

        // GET: Customer/Orders
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Orders.Include(o => o.Users);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Customer/Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Users)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Customer/Orders/Create
        public async Task<IActionResult> Create()
        {
            var user = await _usermanager.GetUserAsync(User) as ApplicationUser;
            var userId = user.Id;
            ViewData["UserId"] = userId;
            //ViewData["OrderType"] = Enum.GetValues(typeof(OrderType)).Cast<OrderType>().Select(e => new SelectListItem
            //{
            //    Value = e.ToString(),
            //    Text = e.ToString()
            //}).ToList();
            return View();
        }


        // POST: Customer/Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,OrderDate,UserId,OrderType,TotalAmount,Status")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", order.UserId);
            return View(order);
        }

        // GET: Customer/Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", order.UserId);
            return View(order);
        }

        // POST: Customer/Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,OrderDate,UserId,OrderType,TotalAmount,Status")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", order.UserId);
            return View(order);
        }

        // GET: Customer/Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Users)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Customer/Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
        //Actual Implementation

        // GET: Display Categories
        [HttpGet]
        public async Task<IActionResult> CreateOrder(int productId)
        {

            var user = await _usermanager.GetUserAsync(User) as ApplicationUser;
            var userId = user.Id;
            ViewData["UserId"] = userId;
            ViewData["OrdersType"] = Enum.GetValues(typeof(OrdersType)).Cast<OrdersType>().Select(e => new SelectListItem
            {
                Value = e.ToString(),
                Text = e.ToString()
            }).ToList();
            
            return View(new CreateOrderViewModel { ProductId=productId  });
        }

        // AJAX: Get Products by Category
        [HttpGet]
        public async Task<JsonResult> GetProducts(int productId)
        {
            var products = await _context.Products
                .Where(p => p.ProductId == productId)
                .FirstOrDefaultAsync();

            
            return Json(products);
        }

        // POST: Create Order
        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var user = await _usermanager.GetUserAsync(User) as ApplicationUser;
                var userId = user.Id;
                ViewData["UserId"] = userId;
                ViewData["OrdersType"] = Enum.GetValues(typeof(OrdersType)).Cast<OrdersType>().Select(e => new SelectListItem
                {
                    Value = e.ToString(),
                    Text = e.ToString()
                }).ToList();

                return View(model);

                
            }

            // Create a new Order
            var order = new Order { 
                OrderType=model.OrderType.ToString(),
                UserId=model.UserId,
                TotalAmount=model.Price*model.Quantity,
                Status="Pending",
                OrderDate = DateTime.Now };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Create OrderDetails entry
            var orderDetail = new OrderDetail
            {
                OrderId = order.OrderId,
                ProductId = model.ProductId,
                Quantity = model.Quantity,
                Price = model.Price
            };

            _context.OrderDetails.Add(orderDetail);
            await _context.SaveChangesAsync();
            TempData["success"] = "Order Placed Successfully";
            return RedirectToAction("UserOrders", "Orders", new { area = "Customer" }); // Redirect after successful order
        }

        public async Task<IActionResult> UserOrders()
        {
            var user = await _usermanager.GetUserAsync(User);
            var userId = user.Id;
            var orders = await _context.Orders
                                .Include(o => o.OrderDetails)
                                    .ThenInclude(od => od.Products)
                                .Where(o => o.UserId == userId)
                                .ToListAsync();

            return View(orders);
        }



    }
}

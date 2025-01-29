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

namespace ImsWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CreditManagementsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CreditManagementsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _userManager= userManager;
            _context = context;
        }

        // GET: Admin/CreditManagements
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.CreditManagements.Include(c => c.Suppliers).Include(c => c.Users);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/CreditManagements/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var creditManagement = await _context.CreditManagements
                .Include(c => c.Suppliers)
                .Include(c => c.Users)
                .FirstOrDefaultAsync(m => m.CreditId == id);
            if (creditManagement == null)
            {
                return NotFound();
            }

            return View(creditManagement);
        }

        // GET: Admin/CreditManagements/Create
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id;
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "Address");
            ViewData["UserId"] = userId;
            return View();
        }

        // POST: Admin/CreditManagements/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: Admin/CreditManagements/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CreditId,UserId,SupplierId,OutstandingBalance,CreditLimit,LastUpdated")] CreditManagement creditManagement)
        {
            // Check if the user already has credit with this supplier
            bool creditExists = await _context.CreditManagements
                .AnyAsync(c => c.UserId == creditManagement.UserId && c.SupplierId == creditManagement.SupplierId);

            if (creditExists)
            {
                TempData["Error"] = "A credit record already exists for this user and supplier.";
                return RedirectToAction(nameof(Index)); // Redirect to list instead of creating duplicate
            }

            if (ModelState.IsValid)
            {
                _context.Add(creditManagement);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Credit record added successfully.";
                return RedirectToAction(nameof(Index));
            }

            // Re-populate the dropdown lists in case of validation failure
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "SupplierName", creditManagement.SupplierId);
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "UserName", creditManagement.UserId);

            return View(creditManagement);
        }

        // GET: Admin/CreditManagements/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var creditManagement = await _context.CreditManagements.FindAsync(id);
            if (creditManagement == null)
            {
                return NotFound();
            }
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "Address", creditManagement.SupplierId);
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", creditManagement.UserId);
            return View(creditManagement);
        }

        // POST: Admin/CreditManagements/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CreditId,UserId,SupplierId,OutstandingBalance,CreditLimit,LastUpdated")] CreditManagement creditManagement)
        {
            if (id != creditManagement.CreditId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(creditManagement);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CreditManagementExists(creditManagement.CreditId))
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
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "Address", creditManagement.SupplierId);
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", creditManagement.UserId);
            return View(creditManagement);
        }

        // GET: Admin/CreditManagements/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var creditManagement = await _context.CreditManagements
                .Include(c => c.Suppliers)
                .Include(c => c.Users)
                .FirstOrDefaultAsync(m => m.CreditId == id);
            if (creditManagement == null)
            {
                return NotFound();
            }

            return View(creditManagement);
        }

        // POST: Admin/CreditManagements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var creditManagement = await _context.CreditManagements.FindAsync(id);
            if (creditManagement != null)
            {
                _context.CreditManagements.Remove(creditManagement);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CreditManagementExists(int id)
        {
            return _context.CreditManagements.Any(e => e.CreditId == id);
        }
    }
}

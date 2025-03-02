using ItiUmplemFrigiderul.Data;
using ItiUmplemFrigiderul.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ItiUmplemFrigiderul.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CategoriesController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            _db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Category
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            SetAccessRights();
            return View(await _db.Categories.ToListAsync());
        }

        [AllowAnonymous]
        public async Task<IActionResult> Show(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var category = await _db.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            category.Prods = GetAllProducts(category);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult New()
        {
            return View();
        }

        // POST: Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Admin")]
        public IActionResult New([Bind("Id,CategoryName")] Category category)
        {
            if (ModelState.IsValid)
            {
                category.Prods = GetAllProducts(category);
                _db.Add(category);
                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _db.Categories.FindAsync(id);
            category.Prods = GetAllProducts(category);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CategoryName")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    category.Prods = GetAllProducts(category);
                    _db.Update(category);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
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
            return View(category);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _db.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }


        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _db.Categories.FindAsync(id);
            _db.Categories.Remove(category);
            TempData["message"] = "Category has been deleted successfully.";
            TempData["messageType"] = "alert-success";
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _db.Categories.Any(e => e.Id == id);
        }

        private void SetAccessRights()
        {
            ViewBag.AfisareButoane = false;

            if (User.IsInRole("Admin"))
            {
                ViewBag.AfisareButoane = true;
            }

            ViewBag.UserCurent = _userManager.GetUserId(User);

            ViewBag.EsteAdmin = User.IsInRole("Admin");
        }

        [NonAction]
        public IEnumerable<Product>? GetAllProducts(Category category)
        {

            var products = _db.Products.Where(p => p.CategoryId == category.Id).ToList();

            return products;
        }
    }
}

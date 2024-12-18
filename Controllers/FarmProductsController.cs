using ItiUmplemFrigiderul.Data;
using ItiUmplemFrigiderul.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ItiUmplemFrigiderul.Controllers
{
    public class FarmProductsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public FarmProductsController(
            ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager
        )
        {
            _db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        public async Task<IActionResult> Show(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fp = await _db.FarmProducts
                .Include("Reviews") 
                .Include("Reviews.User")
                .Include("Farm")
                .Include("Product")
                .FirstOrDefaultAsync(m => m.Id == id);
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }
            if (fp == null)
            {
                return NotFound();
            }

            return View(fp);
        }

        [HttpPost]
        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult Show([FromForm] Review review)
        {
            review.Date = DateTime.Now;

            review.UserId = _userManager.GetUserId(User);
            ModelState.Remove("FarmProduct");
            if (ModelState.IsValid)
            {
                _db.Reviews.Add(review);
                _db.SaveChanges();
                return Redirect("/FarmProducts/Show/" + review.FarmProductId);
            }
            else
            {
                FarmProduct fp = _db.FarmProducts.Include("Product")
                                         .Include("Farm")
                                         .Include("Farm.User")
                                         .Include("Reviews")
                                         .Include("Reviews.User")
                                         .Where(fp => fp.Id == review.FarmProductId)
                                         .First();



                return View(fp);
            }
        }


        public IActionResult New(int? id)
        {
            FarmProduct fp = new FarmProduct();
            //FarmId = id;
            fp.Prod = GetAllProducts();

            return View(fp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult New(FarmProduct fp)
        {
            fp.Id = 0;
            if (ModelState.IsValid)
            {
                fp.Verified = false;
                _db.FarmProducts.Add(fp);
                _db.SaveChanges();
                TempData["message"] = "Product has been added successfully.";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Show", "Farms", new { id = fp.FarmId });
            }
            fp.Prod = GetAllProducts();
            return View(fp);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fp = await _db.FarmProducts.FindAsync(id);
            if (fp == null)
            {
                return NotFound();
            }
            return View(fp);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FarmProduct fp)
        {
            if (id != fp.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _db.Update(fp);
                await _db.SaveChangesAsync();
                
                TempData["message"] = "Product has been edited successfully.";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Show", "Farms", new { id = fp.FarmId });
            }
            return View(fp);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Verify(int id)
        {
            FarmProduct fp = _db.FarmProducts.Find(id);

            try
            {
                fp.Verified = true;
                _db.SaveChanges();
                TempData["message"] = "Product has been verified successfully.";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Show", "Farms",new { id = fp.FarmId });
            }
            catch (DbUpdateConcurrencyException)
            {
                TempData["message"] = "Esti Gras!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Farms");
                throw;
            }
            
            
        }

        //[HttpPost, ActionName("Delete")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var fp = await _db.FarmProducts.FindAsync(id);
            _db.FarmProducts.Remove(fp);
            TempData["message"] = "Product has been deleted successfully.";
            TempData["messageType"] = "alert-success";
            await _db.SaveChangesAsync();
            return RedirectToAction("Show", "Farms", new { id = fp.FarmId });
        }

        [NonAction]
        public IEnumerable<SelectListItem> GetAllProducts()
        {
            // generam o lista de tipul SelectListItem fara elemente
            var selectList = new List<SelectListItem>();

            // extragem toate categoriile din baza de date
            var products = from prod in _db.Products
                             select prod;

            // iteram prin categorii
            foreach (var prod in products)
            {
                selectList.Add(new SelectListItem
                {
                    Value = prod.Id.ToString(),
                    Text = prod.Name
                });
            }
            return selectList;
        }
    }
}

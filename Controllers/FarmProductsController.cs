using ItiUmplemFrigiderul.Data;
using ItiUmplemFrigiderul.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ItiUmplemFrigiderul.Controllers
{
    public class FarmProductsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public FarmProductsController(ApplicationDbContext context)
        {
            _db = context;
        }


        public async Task<IActionResult> Show(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fp = await _db.FarmProducts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fp == null)
            {
                return NotFound();
            }

            return View(fp);
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
                return RedirectToAction("Index", "Farms");
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,CategoryName")] FarmProduct fp)
        {
            if (id != fp.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(fp);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                       throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(fp);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fp = await _db.FarmProducts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fp == null)
            {
                return NotFound();
            }

            return View(fp);
        }


        //[HttpPost, ActionName("Delete")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var fp = await _db.FarmProducts.FindAsync(id);
            _db.FarmProducts.Remove(fp);
            await _db.SaveChangesAsync();
            return Redirect("Farms/Show/" + fp.FarmId);
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

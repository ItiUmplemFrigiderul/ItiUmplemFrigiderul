using ItiUmplemFrigiderul.Data;
using ItiUmplemFrigiderul.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ItiUmplemFrigiderul.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _db = context;
            _userManager = userManager;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int id)
        {
            Cart cart = new Cart();
            cart.FarmProductId = id;
            cart.UserId = _userManager.GetUserId(User);
            cart.Quantity = 1;

            try
            {
                _db.Carts.Add(cart);
                _db.SaveChanges();
                TempData["message"] = "Product has been added to your Cart.";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Index", "Products");
            }
            catch (DbUpdateConcurrencyException)
            {
                TempData["message"] = "Esti Gras!";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Products");
                throw;
            }


        }
    }
}

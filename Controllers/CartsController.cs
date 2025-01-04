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

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var cartItems = await _db.Carts
                                     .Where(c => c.UserId == userId)
                                     .Include(c => c.FarmProduct)
                                         .ThenInclude(fp => fp.Product)
                                     .ToListAsync();

            return View(cartItems);
        }

        public async Task<IActionResult> AddToCart(int farmProductId, float quantity)
        {
            var userId = _userManager.GetUserId(User);
            var existingItem = await _db.Carts
                                        .FirstOrDefaultAsync(c => c.UserId == userId && c.FarmProductId == farmProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                var cartItem = new Cart
                {
                    FarmProductId = farmProductId,
                    Quantity = quantity,
                    UserId = userId
                };
                _db.Carts.Add(cartItem);
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> UpdateCart(int id, float quantity)
        {
            var cartItem = await _db.Carts.FindAsync(id);
            if (cartItem == null || cartItem.UserId != _userManager.GetUserId(User))
            {
                return NotFound();
            }

            cartItem.Quantity = quantity;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var cartItem = await _db.Carts.FindAsync(id);
            if (cartItem == null || cartItem.UserId != _userManager.GetUserId(User))
            {
                return NotFound();
            }

            _db.Carts.Remove(cartItem);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

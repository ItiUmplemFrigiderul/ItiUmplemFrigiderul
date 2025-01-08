using ItiUmplemFrigiderul.Data;
using ItiUmplemFrigiderul.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ItiUmplemFrigiderul.Controllers
{
    [Authorize]
    public class CartsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _db = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "User, Collaborator, Admin")]
        public IActionResult Index()
        {
            var user = _userManager.GetUserId(User);

            var products = _db.Carts
                              .Include(c => c.FarmProduct)
                              .ThenInclude(fp => fp.Product)
                              .Include(c => c.FarmProduct.Farm)
                              .Where(c => c.UserId == user)
                              .ToList();
            int total = products.Sum(p => p.Quantity * p.FarmProduct.Price);

            ViewBag.Total = total;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            ViewBag.CartProducts = products;

            return View(products);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int id)
        {
            var cartItem = new Cart
            {
                FarmProductId = id,
                UserId = _userManager.GetUserId(User),
                Quantity = 1
            };

            try
            {
                _db.Carts.Add(cartItem);
                await _db.SaveChangesAsync();
                TempData["message"] = "Product has been added to your Cart.";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Index", "Products");
            }
            catch (DbUpdateConcurrencyException)
            {
                TempData["message"] = "Failed to add product to cart.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Products");
                throw;
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateQuantity(int farmProductId, int quantity, string submitAction)
        {
            var userId = _userManager.GetUserId(User);
            var cartItem = _db.Carts
                .FirstOrDefault(c => c.FarmProductId == farmProductId && c.UserId == userId);

            if (cartItem == null)
            {
                
                return RedirectToAction(nameof(Index));
            }

           
            switch (submitAction)
            {
                case "increase":
                    cartItem.Quantity = quantity + 1;
                    break;
                case "decrease":
                    cartItem.Quantity = (quantity > 1) ? quantity - 1 : 1;
                    break;
                case "update":
                    
                    cartItem.Quantity = (quantity < 1) ? 1 : quantity;
                    break;
                default:
                    
                    break;
            }

            _db.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FinalizeOrder(string adress)
        {
            
            var userId = _userManager.GetUserId(User);

            
            var cartItems = _db.Carts
                .Include(c => c.FarmProduct)
                .ThenInclude(fp => fp.Product)
                .Where(c => c.UserId == userId)
                .ToList();

           
            if (!cartItems.Any())
            {
                TempData["message"] = "Your cart is empty.";
                TempData["messageType"] = "alert-warning";
                return RedirectToAction("Index");
            }

            
            foreach (var item in cartItems)
            {
                if (item.Quantity > item.FarmProduct.Stock)
                {
                    TempData["message"] = $"Not enough stock for {item.FarmProduct.Product.Name}!";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Index");
                }
            }

            var newOrder = new Order
            {
                UserId = userId,
                Adress = adress, 
                Total = cartItems.Sum(ci => ci.Quantity * ci.FarmProduct.Price),
                Date = DateTime.Now
            };

            _db.Orders.Add(newOrder);
            _db.SaveChanges(); 

            
            foreach (var item in cartItems)
            {
                
                var productOrder = new ProductOrder
                {
                    OrderId = newOrder.Id,
                    FarmProductId = item.FarmProduct.Id,
                    Quantity = item.Quantity
                };
                _db.ProductOrders.Add(productOrder);

                
                item.FarmProduct.Stock -= item.Quantity;

                
                _db.Carts.Remove(item);
            }

            
            await _db.SaveChangesAsync();

            TempData["message"] = "Order finalized successfully!";
            TempData["messageType"] = "alert-success";

           
            return RedirectToAction(nameof(Index));
        }

    }
}

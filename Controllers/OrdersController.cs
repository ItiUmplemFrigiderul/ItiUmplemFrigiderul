using ItiUmplemFrigiderul.Data;
using ItiUmplemFrigiderul.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ItiUmplemFrigiderul.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrdersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _db = context;
            _userManager = userManager;
        }

        // GET: Orders
        [Authorize(Roles = "User,Collaborator,Admin")]
        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User);

            var orders = _db.Orders
                            .Include("User")
                            .Include("ProductOrders")
                            .OrderByDescending(o => o.Date)
                            .Where(ord => ord.UserId == userId)
                            .ToList();

            ViewBag.Orders = orders;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            return View();
        }

        // GET: Orders/Details/5
        [Authorize(Roles = "User,Collaborator,Admin")]
        public IActionResult Show(int id)
        {
            var order = _db.Orders
                           .Include("ProductOrders")
                           .Include("ProductOrders.FarmProduct.Farm")
                           .Include("ProductOrders.FarmProduct.Product")
                           .FirstOrDefault(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }


        [Authorize(Roles = "Admin")]
        public IActionResult ShowAllOrders()
        {
            var orders = _db.Orders
                            .Include(o => o.User)
                            .Include(o => o.ProductOrders)
                            .OrderByDescending(o => o.Date)
                            .ToList();

            return View(orders);
        }


    }
}

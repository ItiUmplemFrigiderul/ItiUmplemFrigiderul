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
            var orders = _db.Orders
                            .Include("User")
                            .Include("ProductOrders")
                            .Include("ProductId")
                            .OrderByDescending(o => o.Date);

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
        public IActionResult Details(int id)
        {
            var order = _db.Orders
                           .Include("ProductOrders")
                           .Include("ProductId")
                           .FirstOrDefault(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        [Authorize(Roles = "User")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User")]
        public IActionResult Create(Order order)
        {
            if (ModelState.IsValid)
            {
                order.UserId = _userManager.GetUserId(User);
                order.Date = DateTime.Now;

                _db.Orders.Add(order);
                _db.SaveChanges();

                TempData["message"] = "Order created successfully!";
                TempData["messageType"] = "alert-success";

                return RedirectToAction(nameof(Index));
            }

            return View(order);
        }

        // GET: Orders/Edit/5
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            var order = _db.Orders.Find(id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id, Order updatedOrder)
        {
            var order = _db.Orders.Find(id);
            if (order == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                order.Adress = updatedOrder.Adress;
                order.Total = updatedOrder.Total;

                _db.SaveChanges();

                TempData["message"] = "Order updated successfully!";
                TempData["messageType"] = "alert-success";

                return RedirectToAction(nameof(Index));
            }

            return View(updatedOrder);
        }

        // POST: Orders/Delete/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var order = _db.Orders.Find(id);
            if (order == null)
            {
                return NotFound();
            }

            _db.Orders.Remove(order);
            _db.SaveChanges();

            TempData["message"] = "Order deleted successfully!";
            TempData["messageType"] = "alert-success";

            return RedirectToAction(nameof(Index));
        }
    }
}

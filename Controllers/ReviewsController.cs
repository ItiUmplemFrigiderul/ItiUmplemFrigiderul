using ItiUmplemFrigiderul.Data;
using ItiUmplemFrigiderul.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ItiUmplemFrigiderul.Controllers
{
    [Authorize]
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _db = context;
            _userManager = userManager;
        }

        // GET: Reviews
        [Authorize(Roles = "User,Collaborator,Admin")]
        public IActionResult Index()
        {
            var reviews = _db.Reviews
                             .Include(r => r.FarmProduct)
                             .ThenInclude(fp => fp.Product)
                             .Include(r => r.User)
                             .OrderByDescending(r => r.Id);

            ViewBag.Reviews = reviews;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            return View();
        }

        // GET: Reviews/Create
        [Authorize(Roles = "User")]
        public IActionResult Create()
        {
            ViewBag.FarmProducts = GetFarmProductsList();
            return View();
        }

        // POST: Reviews/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User")]
        public IActionResult Create(Review review)
        {
            if (ModelState.IsValid)
            {
                review.UserId = _userManager.GetUserId(User);

                _db.Reviews.Add(review);
                _db.SaveChanges();

                TempData["message"] = "Review added successfully!";
                TempData["messageType"] = "alert-success";

                return RedirectToAction(nameof(Index));
            }

            ViewBag.FarmProducts = GetFarmProductsList();
            return View(review);
        }

        // GET: Reviews/Edit/5
        [Authorize(Roles = "User,Collaborator,Admin")]
        public IActionResult Edit(int id)
        {
            var review = _db.Reviews.Find(id);
            if (review == null)
            {
                return NotFound();
            }

            if (review.UserId != _userManager.GetUserId(User) && !User.IsInRole("Admin"))
            {
                TempData["message"] = "You are not authorized to edit this review.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.FarmProducts = GetFarmProductsList();
            return View(review);
        }

        // POST: Reviews/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User,Collaborator,Admin")]
        public IActionResult Edit(int id, Review updatedReview)
        {
            var review = _db.Reviews.Find(id);
            if (review == null)
            {
                return NotFound();
            }

            if (review.UserId != _userManager.GetUserId(User) && !User.IsInRole("Admin"))
            {
                TempData["message"] = "You are not authorized to edit this review.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                review.Rating = updatedReview.Rating;
                review.Content = updatedReview.Content;

                _db.SaveChanges();

                TempData["message"] = "Review updated successfully!";
                TempData["messageType"] = "alert-success";

                return RedirectToAction(nameof(Index));
            }

            ViewBag.FarmProducts = GetFarmProductsList();
            return View(updatedReview);
        }

        // POST: Reviews/Delete/5
        [HttpPost]
        [Authorize(Roles = "User,Collaborator,Admin")]
        public IActionResult Delete(int id)
        {
            var review = _db.Reviews.Find(id);
            if (review == null)
            {
                return NotFound();
            }

            if (review.UserId != _userManager.GetUserId(User) && !User.IsInRole("Admin"))
            {
                TempData["message"] = "You are not authorized to delete this review.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction(nameof(Index));
            }

            _db.Reviews.Remove(review);
            _db.SaveChanges();

            TempData["message"] = "Review deleted successfully!";
            TempData["messageType"] = "alert-success";

            return RedirectToAction(nameof(Index));
        }

        // Helper to fetch farm products for dropdowns
        private IEnumerable<SelectListItem> GetFarmProductsList()
        {
            return _db.FarmProducts
                      .Include(fp => fp.Product)
                      .Select(fp => new SelectListItem
                      {
                          Value = fp.Id.ToString(),
                          Text = fp.Product.Name
                      }).ToList();
        }
    }
}

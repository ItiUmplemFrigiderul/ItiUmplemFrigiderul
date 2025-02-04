﻿using ItiUmplemFrigiderul.Data;
using ItiUmplemFrigiderul.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ItiUmplemFrigiderul.Controllers
{
    public class ReviewsController : Controller
    {

        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public ReviewsController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        
        

        

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Delete(int id)
        {
            Review rev = db.Reviews.Find(id);

            if (rev.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.Reviews.Remove(rev);
                db.SaveChanges();
                return Redirect("/FarmProducts/Show/" + rev.FarmProductId);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti recenzia";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Farms");
            }
        }


        [AllowAnonymous]
        public IActionResult Edit(int id)
        {
            Review rev = db.Reviews.Find(id);

            if (rev.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                return View(rev);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa editati recenzia";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Farms");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Edit(int id, Review requestReview)
        {
            Review rev = db.Reviews.Find(id);

            if (rev.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                ModelState.Remove("FarmProduct");
                if (ModelState.IsValid)
                {
                    rev.Content = requestReview.Content;
                    rev.Rating = requestReview.Rating;

                    db.SaveChanges();

                    return Redirect("/FarmProducts/Show/" + rev.FarmProductId);
                }
                else
                {
                    return View(requestReview);
                }
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa editati recenzia";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Farms");
            }
        }
    }
}
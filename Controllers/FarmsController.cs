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
    public class FarmsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public FarmsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;

        }

        [AllowAnonymous]
        public IActionResult Index()
        {

            var farms = db.Farms.Include("FarmProducts")
                                .Include("User")
                                .OrderBy(f => f.Name)
                                .ToList();
            ViewBag.Farms = farms;
            SetAccessRights();
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            return View(farms);
        }

        [AllowAnonymous]
        public IActionResult Show(int id)
        {
            var farm = db.Farms.Include("FarmProducts")
                               .Include("FarmProducts.Product")
                               .FirstOrDefault(f => f.Id == id);


            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }
            if (farm == null)
            {
                return NotFound("Farm not found");
            }

            SetAccessRights();
            return View(farm);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult New()
        {
            Farm farm = new Farm();

            farm.Colls = GetAllCollaborators();

            return View(farm);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> New(Farm farm)
        {


            if (ModelState.IsValid)
            {
                db.Farms.Add(farm);
                db.SaveChanges();
                TempData["message"] = "Farm has been added successfully.";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Index");
            }

            return View(farm);
        }

        [Authorize(Roles = "Collaborator, Admin")]
        public IActionResult Edit(int id)
        {
            SetAccessRights();
            var farm = db.Farms.FirstOrDefault(f => f.Id == id);
            if (ViewBag.EsteAdmin || farm.UserId == ViewBag.UserCurent)
            {
                if (farm == null)
                {
                    return NotFound("Farm not found");
                }
            }
            return View(farm);
        }

        [HttpPost]
        [Authorize(Roles = "Collaborator, Admin")]
        public async Task<IActionResult> Edit(int id, Farm updatedFarm)
        {
            Farm farm = db.Farms.Find(id);
            SetAccessRights();

            if (ViewBag.EsteAdmin || farm.UserId == ViewBag.UserCurent)
            {
                if (ModelState.IsValid)
                {
                    farm.Name = updatedFarm.Name;
                    farm.PhoneNumber = updatedFarm.PhoneNumber;
                    farm.Adress = updatedFarm.Adress;
                    farm.FarmProducts = updatedFarm.FarmProducts;

                    db.SaveChanges();
                    TempData["message"] = "Farm has been updated successfully.";
                    TempData["messageType"] = "alert-success";
                    return RedirectToAction("Index");
                }
            }
            return View(updatedFarm);
        }

        [HttpPost]
        [Authorize(Roles = "Collaborator, Admin")]
        public IActionResult Delete(int id)
        {
            var farm = db.Farms.FirstOrDefault(f => f.Id == id);
            if (ViewBag.EsteAdmin || farm.User == ViewBag.UserCurent)
            {
                if (farm == null)
                {
                    return NotFound("Farm not found");
                }

                db.Farms.Remove(farm);
                db.SaveChanges();
                TempData["message"] = "Farm has been deleted successfully.";
                TempData["messageType"] = "alert-success";
            }
            return RedirectToAction("Index");
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
        public IEnumerable<SelectListItem> GetAllCollaborators()
        {

            var selectList = new List<SelectListItem>();


            var usersInRole = _userManager.GetUsersInRoleAsync("Collaborator").GetAwaiter().GetResult();

            foreach (var user in usersInRole)
            {
                selectList.Add(new SelectListItem
                {
                    Value = user.Id,
                    Text = user.UserName
                });
            }

            return selectList;
        }

    }
}

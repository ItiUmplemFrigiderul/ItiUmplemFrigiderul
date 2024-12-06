using Ganss.Xss;
using ItiUmplemFrigiderul.Data;
using ItiUmplemFrigiderul.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ItiUmplemFrigiderul.Controllers
{
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


        // GET: ProductsController
        [Authorize(Roles = "User,Collaborator,Admin")]
        public IActionResult Index()
        {
            var farms = db.Farms.Include("ApplicationUser")
                                      .Include("Product")
                                      .Include("FarmProducts")
                                      .OrderByDescending(a => a.Name);

            // ViewBag.OriceDenumireSugestiva
            ViewBag.Farms = farms;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            // MOTOR DE CAUTARE

            var search = "";

            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim(); // eliminam spatiile libere 

                // Cautare in articol (Title si Content)

                List<int> productsIds = db.Farms.Where
                                        (
                                         at => at.Name.Contains(search)
                                        ).Select(a => a.Id).ToList();

                // Cautare in comentarii (Content)
                List<int> productIdsOfReviewsWithSearchString = db.Reviews
                                        .Where
                                        (
                                         c => c.Content.Contains(search)
                                        ).Select(c => (int)c.FarmProduct.Id).ToList();

                // Se formeaza o singura lista formata din toate id-urile selectate anterior
                List<int> mergedIds = productsIds.Union(productIdsOfReviewsWithSearchString).ToList();


                // Lista articolelor care contin cuvantul cautat
                // fie in articol -> Title si Content
                // fie in comentarii -> Content
                farms = db.Farms.Where(farm => mergedIds.Contains(farm.Id))
                                      .Include("Product")
                                      .Include("FarmProducts")
                                      .Include("ApplicationUser")
                                      .OrderByDescending(a => a.Name);

            }

            ViewBag.SearchString = search;

            // AFISARE PAGINATA

            // Alegem sa afisam 3 articole pe pagina
            int _perPage = 8;

            // Fiind un numar variabil de articole, verificam de fiecare data utilizand 
            // metoda Count()

            int totalItems = farms.Count();

            // Se preia pagina curenta din View-ul asociat
            // Numarul paginii este valoarea parametrului page din ruta
            // /Articles/Index?page=valoare

            var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);

            // Pentru prima pagina offsetul o sa fie zero
            // Pentru pagina 2 o sa fie 3 
            // Asadar offsetul este egal cu numarul de articole care au fost deja afisate pe paginile anterioare
            var offset = 0;

            // Se calculeaza offsetul in functie de numarul paginii la care suntem
            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * _perPage;
            }

            // Se preiau articolele corespunzatoare pentru fiecare pagina la care ne aflam 
            // in functie de offset
            var paginatedProducts = farms.Skip(offset).Take(_perPage);


            // Preluam numarul ultimei pagini
            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)_perPage);

            // Trimitem articolele cu ajutorul unui ViewBag catre View-ul corespunzator
            ViewBag.Farms = paginatedProducts;

            // DACA AVEM AFISAREA PAGINATA IMPREUNA CU SEARCH

            if (search != "")
            {
                ViewBag.PaginationBaseUrl = "/Farms/Index/?search=" + search + "&page";
            }
            else
            {
                ViewBag.PaginationBaseUrl = "/Farms/Index/?page";
            }

            return View();
        }

        [Authorize(Roles = "User,Collaborator,Admin")]
        public IActionResult Show(int id)
        {
            Farm farm = db.Farms.Include("Product")
                                .Include("FarmProducts")
                                    .Include("Review")
                                      .Include("ApplicationUser")

                                         .Include("Reviews.User")
                              .Where(prd => prd.Id == id)
                              .First();

            SetAccessRights();
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            return View(farm);
        }

        [HttpPost]
        [Authorize(Roles = "User,Collaborator,Admin")]
        public IActionResult Show([FromForm] Review review)
        {

            // preluam Id-ul utilizatorului care posteaza comentariul
            review.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.Reviews.Add(review);
                db.SaveChanges();
                return Redirect("/Farms/Show/" + review.FarmProductId);
            }
            else
            {
                Farm frm = db.Farms.Include("Product")
                                         .Include("FarmProduct")
                                         .Include("Review")
                                         .Include("Reviews.User")
                                         .Include("ApplicationUser")
                                         .Where(prd => prd.Id == review.FarmProductId)
                                         .First();

                SetAccessRights();
                return View(frm);
            }
        }

        [Authorize(Roles = "Admin")]
        public IActionResult New()
        {
            Farm farm = new Farm();


            return View(farm);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult New(Farm farm)
        {
            var sanitizer = new HtmlSanitizer();

            if (ModelState.IsValid)
            {

                db.Farms.Add(farm);
                db.SaveChanges();
                TempData["message"] = "Ferma a fost adaugata";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Index");
            }
            else
            {
                return View(farm);
            }
        }


        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {

            Farm farm = db.Farms.Include("Product")
                .Include("ApplicationUser")
                                         .Include("FarmPeoducts")
                                         .Where(prd => prd.Id == id)
                                         .First();


            if (User.IsInRole("Admin"))//daca e utilizatorul curent admin
            {
                return View(farm);
            }
            else
            {

                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unei ferme";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id, Farm requestProduct)
        {
            var sanitizer = new HtmlSanitizer();

            Farm farm = db.Farms.Find(id);

            if (ModelState.IsValid)
            {
                if (User.IsInRole("Admin"))
                {
                    farm.Name = requestProduct.Name;
                    farm.UserId = requestProduct.UserId;
                    farm.PhoneNumber = requestProduct.PhoneNumber;
                    farm.Adress = requestProduct.Adress;


                    farm.FarmProducts = requestProduct.FarmProducts;
                    TempData["message"] = "Ferma a fost modificata";
                    TempData["messageType"] = "alert-success";
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unei ferme.";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return View(requestProduct);
            }
        }



        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            // Article farm = db.Articles.Find(id);

            Farm farm = db.Farms.Include("Reviews")
                                         .Include("FarmProducts")
                                         .Where(art => art.Id == id)
                                         .First();

            if (User.IsInRole("Admin"))
            {
                db.Farms.Remove(farm);
                db.SaveChanges();
                TempData["message"] = "Ferma a fost stearsa";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti o ferma";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
        }

        private void SetAccessRights()
        {
            ViewBag.AfisareButoane = false;

            if (User.IsInRole("Admin") || User.IsInRole("Collaborator"))
            {
                ViewBag.AfisareButoane = true;
            }

            ViewBag.UserCurent = _userManager.GetUserId(User);

            ViewBag.EsteAdmin = User.IsInRole("Admin");
        }
    }
}

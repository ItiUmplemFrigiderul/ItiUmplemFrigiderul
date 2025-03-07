﻿using ItiUmplemFrigiderul.Data;
using ItiUmplemFrigiderul.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ganss.Xss;
using System.Net.NetworkInformation;

namespace ItiUmplemFrigiderul.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {

        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _env;
        public ProductsController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IWebHostEnvironment env
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _env = env;
        }


        // GET: ProductsController
        [AllowAnonymous]
        public IActionResult Index()
        {
            var products = db.Products.Include("Category")
                                      .Include("FarmProducts")
                                      .OrderByDescending(a => a.Name)
                                      .ToList();

            // ViewBag.OriceDenumireSugestiva
            ViewBag.Products = products;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

           var search = "";

            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim(); // eliminam spatiile libere 

                // Cautare in articol (Title si Content)

                List<int> productsIds = db.Products.Where
                                        (
                                         at => at.Name.Contains(search)
                                         || at.Category.CategoryName.Contains(search)
                                        )
                                        .Select(a => a.Id).ToList();

                // Lista articolelor care contin cuvantul cautat
                // fie in articol -> Title si Content
                // fie in comentarii -> Content          //mergeIds
                products = db.Products.Where(product => productsIds.Contains(product.Id))
                      .Include("Category")
                      .OrderByDescending(a => a.Name)
                      .ToList();


            }

            ViewBag.SearchString = search;

            // AFISARE PAGINATA

            int _perPage = 6;

            int totalItems = products.Count();

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
            var paginatedProducts = products.Skip(offset).Take(_perPage);


            // Preluam numarul ultimei pagini
            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)_perPage);

            // Trimitem articolele cu ajutorul unui ViewBag catre View-ul corespunzator
            ViewBag.Products = paginatedProducts;

            // DACA AVEM AFISAREA PAGINATA IMPREUNA CU SEARCH

            if (search != "")
            {
                ViewBag.PaginationBaseUrl = "/Products/Index/?search=" + search + "&page";
            }
            else
            {
                ViewBag.PaginationBaseUrl = "/Products/Index/?page";
            }

            return View(products);
        }

        [AllowAnonymous]
        public IActionResult Show(int id)
        {
            Product product =  db.Products.Include("Category")
                              .Include("FarmProducts")
                              .Include("FarmProducts.Farm")
                              .Where(prd => prd.Id == id)
                              .FirstOrDefault();
            ViewBag.Product = product;
            if (product == null)
            {
                // If no product found, redirect to a different page or show an error message
                return NotFound("Product not found");
            }
            SetAccessRights();
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            return View(product);
        }

        //[AllowAnonymous]
        //public IActionResult Show([FromForm] Review review)
        //{

        //    // preluam Id-ul utilizatorului care posteaza comentariul
        //    review.UserId = _userManager.GetUserId(User);

        //    if (ModelState.IsValid)
        //    {
        //        db.Reviews.Add(review);
        //        db.SaveChanges();
        //        return Redirect("/Products/Show/" + review.FarmProductId);
        //    }
        //    else
        //    {
        //        Product prd = db.Products.Include("Category")
        //                                 .Include("FarmProducts")
        //                                 .Include("FarmProducts.Farm")
        //                                 .Where(prd => prd.Id == review.FarmProductId)
        //                                 .First();

        //        SetAccessRights();
        //        return View(prd);
        //    }
        //}

        [Authorize(Roles = "Admin")]
        public IActionResult New()
        {
            Product product = new Product();

            product.Categ = GetAllCategories();

            return View(product);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> New(Product product, IFormFile Photo)
        {
            if (Photo != null && Photo.Length > 0)
            {
                // Verificăm extensia
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".mp4", ".mov" };
                var fileExtension = Path.GetExtension(Photo.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("ProductPhoto", "Fișierul trebuie să fie o imagine(jpg, jpeg, png, gif) sau un video(mp4, mov).");
                return View(product);
                }
               
                // Cale stocare
                var storagePath = Path.Combine(_env.WebRootPath, "images", Photo.FileName);
                var databaseFileName = "/images/" + Photo.FileName;
                // Salvare fișier
                using (var fileStream = new FileStream(storagePath, FileMode.Create))
                {
                    await Photo.CopyToAsync(fileStream);
                }
                ModelState.Remove(nameof(product.Photo));
                product.Photo = databaseFileName;

            }
            if (TryValidateModel(product))
            {
                
                var sanitizer = new HtmlSanitizer();
                product.Description = sanitizer.Sanitize(product.Description);
                db.Products.Add(product);
                db.SaveChanges();
                TempData["message"] = "Produsul a fost adaugat";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Index");
            }
            product.Categ = GetAllCategories();
            return View(product);
        }


        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {

            Product product = db.Products.Include("Category")
                                         .Where(prd => prd.Id == id)
                                         .First();

            product.Categ = GetAllCategories();

            if (User.IsInRole("Admin"))//daca e utilizatorul curent admin
            {
                return View(product);
            }
            else
            {

                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui produs";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Product requestProduct, IFormFile Photo)
        {
            Product product = db.Products.Find(id);
            var sanitizer = new HtmlSanitizer();

            if (Photo != null && Photo.Length > 0)
            {
                // Verificăm extensia
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".mp4", ".mov" };
                var fileExtension = Path.GetExtension(Photo.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("ProductPhoto", "Fișierul trebuie să fie o imagine(jpg, jpeg, png, gif) sau un video(mp4, mov).");
                    return View(product);
                }

                // Cale stocare
                var storagePath = Path.Combine(_env.WebRootPath, "images", Photo.FileName);
                var databaseFileName = "/images/" + Photo.FileName;
                // Salvare fișier
                using (var fileStream = new FileStream(storagePath, FileMode.Create))
                {
                    await Photo.CopyToAsync(fileStream);
                }
                ModelState.Remove(nameof(product.Photo));
                product.Photo = databaseFileName;

            }
            

            if (ModelState.IsValid)
            {
                if ( User.IsInRole("Admin"))
                {
                    
                    product.Name = requestProduct.Name;

                    requestProduct.Description = sanitizer.Sanitize(requestProduct.Description);

                    product.Description = requestProduct.Description;

                    product.CategoryId = requestProduct.CategoryId;
                    product.FarmProducts = requestProduct.FarmProducts;
                    TempData["message"] = "Produsul a fost modificat";
                    TempData["messageType"] = "alert-success";
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui produs.";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                requestProduct.Categ = GetAllCategories();
                return View(requestProduct);
            }
        }



        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            // Article product = db.Articles.Find(id);

            Product product = db.Products
                                         .Where(art => art.Id == id)
                                         .First();

            if (User.IsInRole("Admin"))
            {
                db.Products.Remove(product);
                db.SaveChanges();
                TempData["message"] = "Produsul a fost sters";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti un produs";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
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
        public IEnumerable<SelectListItem> GetAllCategories()
        {
            // generam o lista de tipul SelectListItem fara elemente
            var selectList = new List<SelectListItem>();

            // extragem toate categoriile din baza de date
            var categories = from cat in db.Categories
                             select cat;

            // iteram prin categorii
            foreach (var category in categories)
            {
                selectList.Add(new SelectListItem
                {
                    Value = category.Id.ToString(),
                    Text = category.CategoryName
                });
            }
            return selectList;
        }

    }
}

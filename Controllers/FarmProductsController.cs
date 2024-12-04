using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ItiUmplemFrigiderul.Controllers
{
    public class FarmProductsController : Controller
    {
        // GET: FarmProductsController
        public ActionResult Index()
        {
            return View();
        }

        // GET: FarmProductsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: FarmProductsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FarmProductsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: FarmProductsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: FarmProductsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: FarmProductsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: FarmProductsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}

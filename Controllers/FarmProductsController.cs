using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ItiUmplemFrigiderul.Controllers
{
    public class FarmProductsController : Controller
    {

        // GET: FarmProductsController/Create
        public ActionResult New()
        {
            return View();
        }

        // POST: FarmProductsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult New(IFormCollection collection)
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

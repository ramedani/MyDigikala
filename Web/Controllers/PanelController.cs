using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class PanelController : Controller
    {
        // GET: PanelController
        public ActionResult Index()
        {
            return View();
        }

        // GET: PanelController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }


        public ActionResult dashboard()
        {
            return View();
        }
        public ActionResult sefaresh()
        {
            return View();
        }
        public ActionResult alaghe_mandi()
        {
            return View();
        }



        // GET: PanelController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PanelController/Create
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

        // GET: PanelController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PanelController/Edit/5
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

        // GET: PanelController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PanelController/Delete/5
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

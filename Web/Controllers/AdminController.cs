using Application.DTO;
using Application.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class AdminController : Controller
    {
        ISiteSetting isetting;
        private readonly IProduct iproduct;
        private readonly IComment icomment;
        public AdminController(ISiteSetting _isetting, IProduct _iproduct,IComment _icomment) {
            isetting=_isetting;

            iproduct = _iproduct;
            icomment = _icomment;
        }
                
        //#######################
        //#    Start Comment    #
        //#######################
        public async Task<IActionResult> PendingComments()
        {
            var model = await icomment.GetCommentsForAdminAsync();
            return View(model);
        }

        // 2. تایید نظر
        public async Task<IActionResult> ApproveComment(int id)
        {
            await icomment.ApproveCommentAsync(id);
            // رفرش صفحه برای دیدن تغییرات
            return RedirectToAction("PendingComments");
        }

        // 3. حذف نظر
        public async Task<IActionResult> DeleteComment(int id)
        {
            await icomment.DeleteCommentAsync(id);
            return RedirectToAction("PendingComments");
        }
        public async Task<IActionResult> ListComment()
        {

            var result = await icomment.GetAll();

            return View(result);
        }

        
        
        //#######################
        //#    End Comment    #
        //#######################
        
        
        // GET: AdminController
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Setting()
        {
            return View(isetting.Get());
        }

        [HttpPost]
        public async Task< ActionResult> Setting(SiteSettingDTO obj)
        {
          await isetting.CreatOrUpdate(obj);

            return View(obj);
        }

        // GET: AdminController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AdminController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AdminController/Create
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

        // GET: AdminController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AdminController/Edit/5
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

        // GET: AdminController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AdminController/Delete/5
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

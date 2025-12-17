using Application.DTO;
using Application.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class AdminController : Controller
    {
        ISiteSetting isetting;
        private readonly IProducts iproduct;
        public AdminController(ISiteSetting _isetting, IProducts _iproduct) {
            isetting=_isetting;

            iproduct = _iproduct;
        }
        public async Task<IActionResult> AddProduct(int? id)
        {
            // این متد باید ViewBagها را پر کند
       

            // حالت افزودن محصول جدید
            if (id == null || id == 0)
            {
                // >> مهمترین بخش <<
                // اینجا یک شیء خالی ولی غیر-null به ویو پاس داده می‌شود.
                // نام ویو "AddProduct" است تا با فایل شما مطابقت داشته باشد.
                return View("AddProduct", new CreateProductsDTO());
            }

            // حالت ویرایش محصول موجود
            var productDto = await iproduct.GetForEdit(id.Value);
            if (productDto == null)
            {
                return NotFound();
            }

            // مدل پر شده از دیتابیس به ویو پاس داده می‌شود.
            return View("AddProduct", productDto);
        }

        // این متد کمکی را هم به کنترلر اضافه کنید


        [HttpPost]
        public async Task<IActionResult> AddProduct(CreateProductsDTO dto)
        {
            if (!ModelState.IsValid)
            {
               
                // FIX: نام ویو را به صراحت مشخص کنید
                return View("AddProduct", dto);
            }
            if (dto.Id == 0)
            {
                int newId = await iproduct.CreateProductAsync(dto);
                return RedirectToAction(nameof(AddProduct), new { id = newId });
            }
            else
            {
                await iproduct.Update(dto);
                return RedirectToAction(nameof(AddProduct), new { id = dto.Id });
            }
        }
        public async Task<IActionResult> ProductList()
        {
            // سرویس لیست DTO برمی‌گرداند که بسیار سبک است
            var products = await iproduct.GetAll();
            return View(products);
        }
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await iproduct.Delete(id);
            return RedirectToAction(nameof(ProductList));
        }


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

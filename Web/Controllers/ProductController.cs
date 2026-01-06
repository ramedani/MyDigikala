using System.Diagnostics;
using Application.DTO;
using Application.Interface;
using Infra;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Web.Models;

namespace Web.Controllers
{
    [Route("Admin/Product/[action]")]
	public class ProductController : Controller
	{
        private readonly ILogger<AdminController> _logger;
        private readonly AppDbContext _mydb;
        private IWebHostEnvironment wbh;
        private IBrand ibrand;
        private ICategory icategory;
        private readonly IProduct iproduct;
        private IComment icomment;

		public ProductController( IProduct _iproduct,ICategory _icat, AppDbContext mydb,IWebHostEnvironment _wbh,IBrand _ibrand, ICategory _icategory, IProduct _product, IComment _icomment )
		{
            _mydb = mydb;
            wbh = _wbh;
            ibrand = _ibrand;
            icategory = _icategory;
            iproduct = _product;
            icomment = _icomment;
		}

  
        public async Task<IActionResult> AddProduct(int? id)
        {
            // این متد باید ViewBagها را پر کند
            await PrepareViewBags();

            // حالت افزودن محصول جدید
            if (id == null || id == 0)
            {
                // >> مهمترین بخش <<
                // اینجا یک شیء خالی ولی غیر-null به ویو پاس داده می‌شود.
                // نام ویو "AddProduct" است تا با فایل شما مطابقت داشته باشد.
                return View("AddProduct", new EditProductDto()); 
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
        public async Task<IActionResult> AddProduct(EditProductDto request)
        {
            if (!ModelState.IsValid)
            {
                await PrepareViewBags();
                // FIX: نام ویو را به صراحت مشخص کنید
                return View("AddProduct", request); 
            }
            if (request.Id == 0)
            {
                int newId = await iproduct.Create(request);
                return RedirectToAction(nameof(AddProduct), new { id = newId });
            }
            else
            {
                await iproduct.Update(request);
                return RedirectToAction(nameof(AddProduct), new { id = request.Id });
            }
        }
        private async Task PrepareViewBags()
        {
            ViewBag.CategoryList = (await icategory.GetAll())
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList();
        
            ViewBag.BrandList = (await ibrand.GetAll())
                .Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.NameF }).ToList();
        

        }

// متد مخصوص حذف عکس با AJAX
        [HttpPost]
        public async Task<IActionResult> DeleteImage(int imageId)
        {
            await iproduct.DeleteImage(imageId);
            return Json(new { success = true });
        }
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await iproduct.Delete(id);
            // اصلاح شده: کاربر را به متد Index (لیست محصولات) بفرست
            return RedirectToAction(nameof(ProductList)); 
        }

        [HttpPost]
        public async Task<IActionResult> DeleteGroupProduct(List<int> ids)
        {   
            try
            {
                if (ids == null || !ids.Any())
                {
                    return Json(new { success = false, message = "هیچ موردی انتخاب نشده است." });
                }

                // نام متغیر سرویس خود را چک کنید (iproduct یا _productService)
                // فرض بر این است که در بالای کنترلر IProduct را تزریق کرده‌اید
                await iproduct.DeleteGroup(ids);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "خطا در حذف: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatusGroupProduct(List<int> ids)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return Json(new { success = false, message = "هیچ موردی انتخاب نشده است." });
                }

                await iproduct.ToggleStatusGroup(ids);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "خطا در تغییر وضعیت: " + ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdatePriceGroup(List<int> ids, double percentage)
        {
            try
            {
                // 1. بررسی ورودی‌ها
                if (ids == null || !ids.Any())
                {
                    return Json(new { success = false, message = "هیچ محصولی انتخاب نشده است." });
                }

                // 2. فراخوانی متد آماده‌ای که در ProductService نوشتید
                // این متد خودش عملیات محاسبه، رند کردن و ذخیره در دیتابیس را انجام می‌دهد
                await iproduct.UpdatePriceGroup(ids, percentage);

                // 3. بازگرداندن نتیجه موفقیت‌آمیز
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // ثبت خطا (اختیاری)
                // _logger.LogError(ex, "Error updating prices");
        
                return Json(new { success = false, message = "خطایی رخ داد: " + ex.Message });
            }
        }
        public async Task<IActionResult> ProductList()
        {
            ViewBag.Stats = await iproduct.GetDashboardStatsAsync();

            var mostViewed = await iproduct.GetMostViewedProduct(); 

            if (mostViewed != null)
            {
                ViewBag.MostViewedTitle = mostViewed.Title;
                ViewBag.MostViewedCount = mostViewed.VisitCount;
            }
            else
            {
                ViewBag.MostViewedTitle = "محصولی یافت نشد";
                ViewBag.MostViewedCount = 0;
            }
            // سرویس لیست DTO برمی‌گرداند که بسیار سبک است
            var products = await iproduct.GetAll();
            return View(products);
        }


      
        public async Task<IActionResult> AddBrand(int? id)
        {
            if (id == null)
                return View("AddBrand", new EditBrandDto());

            var obj = await ibrand.Get(id.Value);
            if (obj == null)
                return NotFound();

            return View("AddBrand", obj);
        }
        [HttpPost]
        public async Task<IActionResult> AddBrand(EditBrandDto obj)
        {
            if (!ModelState.IsValid)
                return View(obj);

            if (obj.Id == 0)
                await ibrand.Create(obj);
            else
                await ibrand.Update(obj);

            return RedirectToAction("AddProduct");
        }


        public async Task<IActionResult> BrandList()
        {
            var list = await ibrand.GetAll();
            return View(list);
        }

        public async Task<IActionResult> DeleteBrand(int id)
        {
            var result = await ibrand.Delete(id);

            // برند محصول دارد → صفحه مخصوص را نمایش بده
            if (!result.Success && result.HasProducts)
            {
                return View("BrandDeleteBlocked", result.BlockInfo);
            }

            if (!result.Success)
                return NotFound();

            TempData["Success"] = "برند با موفقیت حذف شد.";
            return RedirectToAction("BrandList");
        }
        public async Task<IActionResult> AddCategory(int? id)
        {
            if (id == null)
                return View("AddCategory", new EditCategoryDto());

            var obj = await icategory.Get(id.Value);
            if (obj == null)
                return NotFound();

            return View("AddCategory", obj);
        }
        [HttpPost]
        public async Task<IActionResult> AddCategory(EditCategoryDto obj)
        {
            if (!ModelState.IsValid)
                return View(obj);

            if (obj.Id == 0)
                await icategory.Create(obj);
            else
                await icategory.Update(obj);

            return RedirectToAction("AddProduct");
        }

        
        public async Task<IActionResult> CategoryList()
        {
            var list = await icategory.GetAll();
            return View(list);
        }

        public async Task<IActionResult> DeleteCategory(int id)
        {
            var result = await icategory.Delete(id);

            // برند محصول دارد → صفحه مخصوص را نمایش بده
            if (!result.Success && result.HasProducts)
            {
                return View("CategoryDeleteBlocked", result.BlockInfo);
            }

            if (!result.Success)
                return NotFound();

            TempData["Success"] = "برند با موفقیت حذف شد.";
            return RedirectToAction("CategoryList");
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
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





        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

	}
}

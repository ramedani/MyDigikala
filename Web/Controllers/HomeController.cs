using Application.DTO;
using Application.Interface;
using Application.Resource;
using Domain;
using Infra;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Web.Models;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private ICategory icat;
        private AppDbContext mydb;
        private readonly INews inews;
        private readonly IMemoryCache _cache;
        private readonly IStringLocalizer<ValidationMessages> _localizer;
        private readonly IDataProtector _protector;
        private readonly ICategory _icategory;
        private readonly IProduct _iproduct;
        private readonly IComment _icomment;
        public HomeController(IDataProtectionProvider provider, IStringLocalizer<ValidationMessages> localizer,ILogger<HomeController> logger , ICategory _icat,AppDbContext _mydb, INews _inews, IMemoryCache cache,ICategory icategory,IProduct iproduct,IComment icomment)
        {
            _logger = logger;
            _localizer = localizer;
            icat = _icat;
            mydb = _mydb;
            inews = _inews;
            _cache = cache;
            _protector =  provider.CreateProtector("hamid");
            _icategory = icategory;
            _iproduct = iproduct;
            _icomment = icomment;   
        }



        //  /home/edata?txt=salam

        //CfDJ8Fru9UbZ5otCsJhe1z96wrPDxUdQ0v1cW-MplDwkU0pnfnZHkPPZ42iXHjpIiKEqs5di2ZoCyCU4vLcCulFhAiM2-kZ-W7ZsoyHqGpzJOI16LLB4gJ_xCB5oHwFtsmvAdQ
        public IActionResult edata(string txt)
        {
            
            string encryptedText = _protector.Protect(txt);
            return Ok(new
            {
                OriginalText = txt,
                EncryptedText = encryptedText
            });
        }

  
        public IActionResult ddata(string txt)
        {
            try
            {
                string decryptedText = _protector.Unprotect(txt);
                return Ok(new
                {
                    DecryptedText = decryptedText,
                });
            }
            catch (Exception)
            {
                return BadRequest("کلید نامعتبر است یا داده دستکاری شده است.");
            }
        }

        // ==========================================================
        // بخش دوم: هش کردن و مقایسه (Hashing / Verification)
        // ==========================================================

        //https://localhost:7207/home/GenerateHash?text=salam
        public IActionResult GenerateHash(string text)
        {

            string textHash = ComputeSha256Hash(text);

            return Ok(new
            {
                TextHash = textHash,
            });
        }


        public IActionResult VerifyHash(string rawText, string hashedText)
        {
            // روش مقایسه: متن ورودی جدید را هش می‌کنیم
            string newHash = ComputeSha256Hash(rawText);

            // هش جدید را با هش قدیمی مقایسه می‌کنیم
            if (newHash == hashedText)
            {
                return Ok("اطلاعات صحیح است و دستکاری نشده.");
            }
            else
            {
                return BadRequest("اطلاعات مطابقت ندارد!");
            }
        }

        // متد کمکی برای تولید SHA256
        private string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // تبدیل رشته به بایت
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // تبدیل بایت به رشته Hex
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }




        ///Home/TestLang?culture=fa
        public IActionResult TestLang()
        {
            // این خط باید متن فارسی را برگرداند
            var text = _localizer["test"];
            return Content($"Current Culture: {CultureInfo.CurrentCulture.Name} | Value: {text}");
        }

        public IActionResult me()
        {
           
            return View(new RegisterUserDto());
        }




        [OutputCache(Duration = 30)]
        public IActionResult GetProduct1s()
        {
            Thread.Sleep(10000);
            return Ok(new[] { "Laptop", "Mouse", "Keyboard" });
        }


        [ResponseCache(Duration = 30)]
        public IActionResult GetProduct7s()
        {
            Thread.Sleep(10000);
            return Ok(new[] { "Laptop", "Mouse", "Keyboard" });
        }

        public IActionResult GetProduct()
        {
            string cacheKey = "current_time";

            // تلاش برای دریافت مقدار از کش
            if (!_cache.TryGetValue(cacheKey, out DateTime cacheValue))
            {
                // اگر در کش نبود، مقدار جدید را تولید می‌کنیم (مثلاً از دیتابیس می‌گیریم)
                cacheValue = DateTime.Now;

                // تنظیمات کش (مثلاً زمان انقضا)
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(30)); // اگر تا ۳۰ ثانیه استفاده نشود پاک می‌شود

                // ذخیره در کش
                _cache.Set(cacheKey, cacheValue, cacheEntryOptions);
            }

            return Ok($"Time: {cacheValue}");
        }


        public async Task<IActionResult> Index()
        {

            ViewBag.Categories = await _icategory.GetAll(); 
            var products = await _iproduct.GetLastProducts(6);
            ViewBag.LatestProducts = products ?? new List<Application.DTO.ProductListDto>();
            ViewBag.AmazingProducts = await _iproduct.GetAmazingProducts(8); 


            var hotNews = await inews.GetHotArticlesForHomePageAsync();
    
            // می‌ریزیم داخل ویوبگ (اگر نال بود یک لیست خالی می‌فرستیم که ارور ندهد)
            ViewBag.HotArticles = hotNews ?? new List<Application.DTO.NewsCardDto>();

            return View();
        }
 
   
        [Route("Home/Product/{id}")] 
        public async Task<IActionResult> Shop(int id)
        {
  
            await _iproduct.IncrementProductVisitAsync(id);

            string? currentUserId = null;
    

            if (User.Identity.IsAuthenticated)
            {
                currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        

                if (string.IsNullOrEmpty(currentUserId))
                {
                    currentUserId = User.FindFirst("sub")?.Value 
                                    ?? User.FindFirst("Id")?.Value
                                    ?? User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value; 
                }
            }


            var model = await _iproduct.GetProductDetailAsync(id, currentUserId);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddReview([FromBody] AddCommentDto dto)
        {
            if (!ModelState.IsValid)
            {
                // جمع‌آوری تمام خطاهای اعتبارسنجی
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var errorMessage = string.Join(" - ", errors);

                // بازگشت خطای دقیق به کاربر
                return Json(new { success = false, message = errorMessage });
            }

            dto.UserId = null; 
            await _icomment.AddCommentAsync(dto);

            return Json(new { success = true });
        }

        /*
        [HttpPost]
        public async Task<IActionResult> VoteComment(int commentId, bool isLike)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { success = false, requireLogin = true });
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
            {
                return Json(new { success = false, message = "User ID invalid" });
            }


            await _iproduct.ToggleVoteAsync(commentId, userId, isLike);

            var counts = await _iproduct.GetCommentCountsAsync(commentId);

            return Json(new {
                success = true,
                likes = counts.likes,
                dislikes = counts.dislikes
            });
        }
        */

        public ActionResult aboutus()
        {
            var setting=mydb.SiteSettings.FirstOrDefault();
            return View(setting);
        }

        
        
        public async Task<IActionResult> Article(
            int pageId = 1, 
            List<int>? selectedCategories = null, 
            bool isFeatured = false, //داخ ترین
            bool isNewest = false)  //بروز ترین
        {
           
            var result = await inews.GetNewsForIndex(pageId, selectedCategories, isFeatured, isNewest);
            var categories = await inews.GetCategoriesWithCountsAsync();


            int pageSize = isNewest ? 10 : 6; 
    
            int totalItems = result.Item2;
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);


            var finalModel = new NewsSearchViewModel
            {
                NewsList = result.Item1,           
                Categories = categories,           
                SelectedCategories = selectedCategories ?? new List<int>(), 
                IsFeatured = isFeatured,
                IsNewest = isNewest,
        

                CurrentPage = pageId,
        
                TotalPages = totalPages
            };


            return View(finalModel);
        }

        [Route("Home/News/{id}")]
        public async Task<IActionResult> News(int id)
        {

            var model = await inews.GetNewsDetailAsync(id);

            if (model == null)
            {
                return NotFound();
            }


            return View(model);
        }



        public IActionResult Privacy()
        {
            return View();
        }



		[Route("Home/Error/{statusCode?}")]
		public IActionResult Error(int? statusCode)
        {

             if (statusCode == 404)
            {
                // نمایش ویوی اختصاصی ۴۰۴
                return View("NotFound");
            }
            else if (statusCode == 400)
            {
                return View("forhundred");
            }

            else if (statusCode == 403)
            {
                return View("forhundredandtwo");
            }

            else if (statusCode == 405)
            {
                return View("forhundredandfive");
            }
            else if (statusCode == 408)
            {
                return View("forhundredandeight");
            }
            else if (statusCode == 409)
            {
                return View("forhundredandnine");
            }

            else if (statusCode == 410)
            {
                return View("forhundredandten");
            }

            else if (statusCode == 422)
            {
                return View("forhundredandtwentytwo");
            }

            else if (statusCode == 429)
            {
                return View("forhundredandtwentynine");
            }

            else if (statusCode == 500)
            {
                return View("fivehundred");
            }

            else if (statusCode == 501)
            {
                return View("fivehundredandone");
            }

            else if (statusCode == 502)
            {
                return View("fivehundredandtwo");
            }

            else if (statusCode == 503)
            {
                return View("fivehundredandthree");
            }

            else if (statusCode == 504)
            {
                return View("fivehundredandfour");
            }

            // نمایش ویوی خطای عمومی (برای ۵۰۰ و سایر خطاها)
            return View("Error");
		}


    }
}
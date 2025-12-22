using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Web.Models;
using Application.DTO;
using Application.Interface;
using System.Threading.Tasks;
using Infra;
namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private ICategory icat;
        private AppDbContext mydb;
        private readonly INews inews;
        public HomeController(ILogger<HomeController> logger , ICategory _icat,AppDbContext _mydb, INews _inews)
        {
            _logger = logger;
            icat = _icat;
            mydb = _mydb;
            inews = _inews;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Shop()
        {
            return View();
        }
        public ActionResult aboutus()
        {
            var setting=mydb.SiteSettings.FirstOrDefault();
            return View(setting);
        }
        public ActionResult CrCt()
        {
            CreateCategoryDTO obj = new CreateCategoryDTO();
            return View(obj);
        }
        public async Task<IActionResult> Article(int page = 1, string sort = "new")
        {
            var result = await inews.GetNewsForIndex(page, sort);

            // داده‌های لیست محصولات
            var model = result.Item1;
        
            // محاسبه تعداد صفحات برای Pagination
            int totalItems = result.Item2;
            int pageSize = 6;
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            // ارسال اطلاعات به ویو
            ViewBag.Page = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Sort = sort;

            return View(model);
        }
        [Route("Home/News/{id}")]
        public async Task<IActionResult> News(int id)
        {
            // تمام کارها در سرویس انجام می‌شود
            var model = await inews.GetNewsDetailAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            // مدل را به ویو می‌فرستیم
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> CrCt(CreateCategoryDTO obj)
        {
            await icat.Creat(obj);
            return View(obj);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
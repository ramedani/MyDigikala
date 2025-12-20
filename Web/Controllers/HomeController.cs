using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Web.Models;
using Application.DTO;
using Application.Interface;
using System.Threading.Tasks;
using Infra;
using Microsoft.AspNetCore.Http;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private ICategory icat;
        private AppDbContext mydb;
        public HomeController(ILogger<HomeController> logger , ICategory _icat,AppDbContext _mydb)
        {
            _logger = logger;
            icat = _icat;
            mydb = _mydb;
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



		[Route("Home/Error/{statusCode?}")]
		public IActionResult Error(int? statusCode)
        {
			if (statusCode == 404)
			{
				// نمایش ویوی اختصاصی ۴۰۴
				return View("NotFound");
			}

			// نمایش ویوی خطای عمومی (برای ۵۰۰ و سایر خطاها)
			return View("Error");
		}


    }
}
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
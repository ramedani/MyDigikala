using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Web.Models;
using Application.DTO;
using Application.Interface;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private ICategory icat;

        public HomeController(ILogger<HomeController> logger,ICategory _icat)
        {
            _logger = logger;
            icat = _icat;
        }


        public ActionResult CrCt()
        {
            CreateCategoryDto obj = new CreateCategoryDto();
            return View(obj);
        }

        [HttpPost]
        public async Task<ActionResult> CrCt(CreateCategoryDto obj)
        {
           
            await icat.Create(obj);
            return View(obj);
        }



        public IActionResult Index()
        {
            return View();
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
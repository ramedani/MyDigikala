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
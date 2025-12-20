using Application.DTO;
using Microsoft.AspNetCore.Mvc;


using Application.DTO;
using Application.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace Web.Controllers;

 
    public class NewsController : Controller
    {
        private readonly INews _news;
        private readonly INewsCategory _newscategory;

        public NewsController(INews news, INewsCategory newsCategory)
        {
            _news = news;
            _newscategory = newsCategory;
        }


        public async Task<IActionResult> ManageNews(int? id)
        {
            
            // حالت افزودن محصول جدید
            if (id == null || id == 0)
            {
                return View("ManageNews", new EditNewsDto());
            }

            // حالت ویرایش محصول موجود
            var newsDto = await _news.GetForEdit(id.Value);
            if (newsDto == null)
            {
                return NotFound();
            }

            return View("ManageNews", newsDto);
        }
        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageNews(EditNewsDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View("ManageNews", dto);
            }
            if (dto.Id == 0)
            {
                int newId = await _news.CreateNewsAsync(dto);
                return RedirectToAction(nameof(ManageNews), new { id = newId });
            }
            else
            {
                await _news.Update(dto);
                return RedirectToAction(nameof(ManageNews), new { id = dto.Id });
            }
            
        }
        
    }

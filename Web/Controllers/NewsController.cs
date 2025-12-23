using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Domain;
using Infra;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web.Models;
using Application.DTO;

using Application.DTO;
using Application.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace Web.Controllers;

 
    public class NewsController : Controller
    {
        private readonly AppDbContext _mydb;
        private readonly INews _news;
        private readonly INewsCategory _newscategory;

        public NewsController(INews news, INewsCategory newsCategory,AppDbContext mydb)
        {
            _news = news;
            _newscategory = newsCategory;
            _mydb = mydb;
        }

        
        public async Task<IActionResult> ManageNews(int? id)
        {
            //View Model
            await PrepareViewBags();
            // حالت اضافه کردن خبر
            if (id == null || id == 0)
            {
                return View("ManageNews", new EditNewsDto());
            }

            // حالت ویرایش خبر موجود
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
                await PrepareViewBags();
                return View("ManageNews", dto);
            }
            //اضافه کردن خبر
            if (dto.Id == 0)
            {
                int newId = await _news.CreateNewsAsync(dto);
                return RedirectToAction(nameof(ManageNews), new { id = newId });
            }
            // ویرایش خبر
            else
            {
                await _news.Update(dto);
                return RedirectToAction(nameof(ManageNews), new { id = dto.Id });
            }
            
        }
        public async Task<IActionResult> ListNews(string search, int? categoryId)
        {
            
            ViewBag.Stats = await _news.GetDashboardStatsAsync();
            
            var newsList = await _news.GetAll();

            return View(newsList);
        }

  
        private async Task PrepareViewBags()
        {
            ViewBag.NewsCategoryList = (await _newscategory.GetAll())
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList();
        

        }

        [HttpPost]
        public async Task<IActionResult> DeleteGroupNews(List<int> ids)
        {   
            try
            {
                if (ids == null || !ids.Any())
                {
                    return Json(new { success = false, message = "هیچ موردی انتخاب نشده است." });
                }

                await _news.DeleteGroup(ids);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "خطا در حذف: " + ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> ChangeStatusGroupNews(List<int> ids)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return Json(new { success = false, message = "هیچ موردی انتخاب نشده است." });
                }

                await _news.ToggleStatusGroup(ids);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "خطا در تغییر وضعیت: " + ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> ChangeFeaturedGroupNews(List<int> ids)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return Json(new { success = false, message = "هیچ موردی انتخاب نشده است." });
                }

                await _news.ToggleFeaturedGroup(ids);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "خطا در تغییر وضعیت: " + ex.Message });
            }
        }
        
        
        
        
        
        //#################
        //#    Ctegory    #
        //#################
        public async Task<IActionResult> AddNewsCategory(int? id)
        {
            if (id == null)
                return View("AddNewsCategory", new EditNewsCategoryDto());

            var obj = await _newscategory.Get(id.Value);
            if (obj == null)
                return NotFound();

            return View("AddNewsCategory", obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddNewsCategory(EditNewsCategoryDto obj)
        {
            if (!ModelState.IsValid)
                return View(obj);

            if (obj.Id == 0)
                await _newscategory.Create(obj);
            else
                await _newscategory.Update(obj);

            return RedirectToAction("ManageNews");
        }

        
        public async Task<IActionResult> NewsCategoryList()
        {
            var list = await _newscategory.GetAll();
            return View(list);
        }

        public async Task<IActionResult> DeleteNewsCategory(int id)
        {
            var result = await _newscategory.Delete(id);
            
            if (!result.Success && result.HasProducts)
            {
                return View("CategoryDeleteBlocked", result.BlockInfo);
            }

            if (!result.Success)
                return NotFound();

            TempData["Success"] = "برند با موفقیت حذف شد.";
            return RedirectToAction("CategoryList");
        }

        
    }

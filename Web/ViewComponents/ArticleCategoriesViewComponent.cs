using Microsoft.AspNetCore.Mvc;
using Infra;
using Microsoft.EntityFrameworkCore;

namespace Web.ViewComponents;

public class ArticleCategoriesViewComponent : ViewComponent
{
    private readonly AppDbContext _mydb; // یا هر نامی که کانتکست دیتابیس شما دارد

    public ArticleCategoriesViewComponent(AppDbContext mydb)
    {
        _mydb = mydb;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        // دریافت دسته‌بندی‌ها مستقل از اینکه در چه صفحه‌ای هستیم
        var categories = await _mydb.NewsCategories
            .OrderByDescending(c => c.Id)
            .ToListAsync();

        return View(categories);
    }
}
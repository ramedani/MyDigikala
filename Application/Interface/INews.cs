using Application.DTO; 
using Domain;         
using Infra;
using Microsoft.AspNetCore.Hosting;
using Application.Helper;
using Microsoft.EntityFrameworkCore;

namespace Application.Interface;

public interface INews
{
    Task<int> CreateNewsAsync(CreatNewsDto dto);
    Task<EditNewsDto> GetForEdit(int id);
    Task Update(EditNewsDto dto);
    Task<List<NewsListDto>> GetAll(string? search = null, int? categoryId = null);
    Task<NewsDashboardStatsDto> GetDashboardStatsAsync();
    
    
    Task DeleteGroup(List<int> ids);
    Task ToggleStatusGroup(List<int> ids);
    Task ToggleFeaturedGroup(List<int> ids);
    Task<Tuple<List<NewsCardDto>, int>> GetNewsForIndex(int pageId = 1, string sort = "newest");
    Task<NewsDetailViewModelDto> GetNewsDetailAsync(int id);
    
}

public class NewsService : INews
{
    private readonly AppDbContext _mydb;
    private readonly IWebHostEnvironment _env;
    private readonly IFileSecurityHelper _fileHelper;

    public NewsService(AppDbContext mydb, IWebHostEnvironment env, IFileSecurityHelper fileHelper)
    {
        _mydb = mydb;
        _fileHelper = fileHelper;
        _env = env;
    }

    //#####################
    //     START Admin
    //#####################  
    public async Task<int> CreateNewsAsync(CreatNewsDto dto)
    {

        var article = new News
        {
            Title = dto.Title,
            AuthorName = dto.AuthorName,
            IsActive = dto.IsActive,
            IsFeatured = dto.IsFeatured,
            NewsCategoryId = (dto.NewsCategoryId == 0) ? null : dto.NewsCategoryId,

            CreateTime = DateTime.UtcNow,
            NewsBlocks = new List<NewsBlocks>()
        };

        if (dto.NewsImageMain != null && dto.NewsImageMain.Length > 0)
        {
            try
            {
                // پوشه ذخیره سازی: wwwroot/NewsImages
                var fileName = await _fileHelper.SecureUploadAsync(dto.NewsImageMain, _env.WebRootPath, "NewsImages");
                article.PicUrl = fileName;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Image Upload Failed: {ex.Message}");
            }
        }

        if (dto.NewsBlocks != null && dto.NewsBlocks.Any())
        {
            foreach (var blockDto in dto.NewsBlocks)
            {
                article.NewsBlocks.Add(new NewsBlocks
                {
                    Content = blockDto.Content,
                    SortOrder = blockDto.SortOrder,
                    BlockType = blockDto.BlockType
                });
            }
        }


        await _mydb.News.AddAsync(article);
        await _mydb.SaveChangesAsync();


        return article.Id;
    }

    public async Task<EditNewsDto?> GetForEdit(int id)
    {
        var news = await _mydb.News
            .Include(n => n.NewsBlocks)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (news == null) return null;

        return new EditNewsDto
        {
            Id = news.Id,
            Title = news.Title,
            AuthorName = news.AuthorName,
            NewsCategoryId = news.NewsCategoryId,
            IsFeatured = news.IsFeatured,
            IsActive = news.IsActive,
            CurrentPicUrl = news.PicUrl,
            
            NewsImageMain = null,

            NewsBlocks = news.NewsBlocks
                .OrderBy(b => b.SortOrder)
                .Select(b => new CreateArticleBlockDto
                {
                    Content = b.Content,
                    BlockType = b.BlockType,
                    SortOrder = b.SortOrder
                }).ToList()
        };
    }


    public async Task Update(EditNewsDto dto)
    {
        var news = await _mydb.News
            .Include(n => n.NewsBlocks)
            .FirstOrDefaultAsync(p => p.Id == dto.Id);

        if (news == null) return;

        news.Title = dto.Title;
        news.AuthorName = dto.AuthorName;
        news.IsFeatured = dto.IsFeatured;
        news.IsActive = dto.IsActive;

        if (dto.NewsCategoryId.HasValue)
        {
            news.NewsCategoryId = dto.NewsCategoryId.Value;
        }

        news.UpdatedAt = DateTime.UtcNow;


        if (dto.NewsImageMain != null && dto.NewsImageMain.Length > 0)
        {
            try
            {
                var fileName = await _fileHelper.SecureUploadAsync(dto.NewsImageMain, _env.WebRootPath, "NewsImages");
                news.PicUrl = fileName;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        if (news.NewsBlocks != null && news.NewsBlocks.Any())
        {
            _mydb.NewsBlocks.RemoveRange(news.NewsBlocks);
        }

        if (dto.NewsBlocks != null && dto.NewsBlocks.Any())
        {
            foreach (var blockDto in dto.NewsBlocks)
            {
                news.NewsBlocks.Add(new NewsBlocks
                {
                    Content = blockDto.Content,
                    BlockType = blockDto.BlockType,
                    SortOrder = blockDto.SortOrder
                });
            }
        }

        await _mydb.SaveChangesAsync();
    }


    public async Task<List<NewsListDto>> GetAll(string? search = null, int? categoryId = null)
    {
        var query = _mydb.News.Include(n => n.NC).AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(n => n.Title.Contains(search) || n.AuthorName.Contains(search));
        }

        if (categoryId.HasValue && categoryId.Value > 0)
        {
            query = query.Where(n => n.NewsCategoryId == categoryId.Value);
        }

        var list = await query.OrderByDescending(n => n.CreateTime)
            .Select(n => new NewsListDto
            {
                Id = n.Id,
                Title = n.Title,
                AuthorName = n.AuthorName,
                CategoryName = n.NC != null ? n.NC.Name : "بدون دسته",
                PicUrl = n.PicUrl,
                IsActive = n.IsActive,
                IsFeatured = n.IsFeatured,
                CreateTime = n.CreateTime ?? DateTime.UtcNow
            }).ToListAsync();

        return list;
    }


    public async Task<NewsDashboardStatsDto> GetDashboardStatsAsync()
    {
        var total = await _mydb.News.CountAsync();
        var active = await _mydb.News.CountAsync(n => n.IsActive);

        return new NewsDashboardStatsDto
        {
            TotalNews = total,
            PublishedCount = active,
            DraftCount = total - active,

        };
    }

    public async Task DeleteGroup(List<int> ids)
    {
        var News = await _mydb.News.Where(p => ids.Contains(p.Id)).ToListAsync();
        _mydb.News.RemoveRange(News);
        await _mydb.SaveChangesAsync();
    }



    public async Task ToggleStatusGroup(List<int> ids)
    {
        var News = await _mydb.News.Where(p => ids.Contains(p.Id)).ToListAsync();
        foreach (var p in News)
        {
            p.IsActive = !p.IsActive;
        }

        await _mydb.SaveChangesAsync();
    }

    public async Task ToggleFeaturedGroup(List<int> ids)
    {
        var News = await _mydb.News.Where(p => ids.Contains(p.Id)).ToListAsync();
        foreach (var p in News)
        {
            p.IsFeatured = !p.IsFeatured;
        }

        await _mydb.SaveChangesAsync();
    }

//#####################
//     END Admin
//#####################  
//#####################
//     START User
//#####################  
    public async Task<Tuple<List<NewsCardDto>, int>> GetNewsForIndex(int pageId = 1, string sort = "new")
    {
        int take = 6; 
        int skip = (pageId - 1) * take;


        var query = _mydb.News
            .AsNoTracking() 
            .Where(p => p.IsActive); 

 
        switch (sort)
        {
            case "new":
                query = query.OrderByDescending(p => p.CreateTime);
                break;
            default: // newest
                query = query.OrderByDescending(p => p.Id); // یا p.CreateDate
                break;
        }

        int totalCount = await query.CountAsync();

        var products = await query
            .Skip(skip)
            .Take(take)

            .Select(p => new NewsCardDto()
            {
                Id = p.Id,
                Title = p.Title,
                CreateTime = p.CreateTime ?? DateTime.UtcNow,
                CategoryName = p.NC != null ? p.NC.Name : "بدون دسته‌بندی",
                PicUrl = p.PicUrl ?? "default.png",


            })

            .ToListAsync();

        return Tuple.Create(products, totalCount);
    }

public async Task<NewsDetailViewModelDto> GetNewsDetailAsync(int id)
{
    // ۱. کوئری اصلی خبر
    var news = await _mydb.News
        .Include(p => p.NC) // فقط ریلیشن‌ها اینکلود می‌شوند
        .FirstOrDefaultAsync(p => p.Id == id);

    if (news == null) return null;

    // ۲. دریافت بلاک‌ها (اصلاح شده برای رفع ارور نال‌پذیری)
    var blocks = await _mydb.NewsBlocks
        .Where(b => b.NewsId == id)
        .OrderBy(b => b.SortOrder)
        .Select(b => new NewsBlockViewModelDto
        {
            // از عملگر ؟؟ استفاده می‌کنیم تا اگر نال بود، مقدار پیش‌فرض جایگزین شود
            Content = b.Content ?? "",       // اگر متن نال بود، رشته خالی بگذار
            BlockType = b.BlockType ?? 1,    // اگر نوع بلاک نال بود، پیش‌فرض ۱ (مثلاً پاراگراف) بگذار
            SortOrder = b.SortOrder ?? 0     // اگر ترتیب نال بود، ۰ بگذار
        })
        .ToListAsync();

    // ۳. مقالات مرتبط
    List<NewsListDto> relatedNewsDto = new List<NewsListDto>();
    if (news.NC != null)
    {
        relatedNewsDto = await _mydb.News
            .Where(p => p.NC.Id == news.NC.Id && p.Id != news.Id)
            .OrderByDescending(p => p.Id)
            .Take(4)
            .Select(p => new NewsListDto
            {
                Id = p.Id,
                Title = p.Title,
                IsActive = p.IsActive,
                CreateTime = p.CreateTime ?? DateTime.UtcNow,
                PicUrl = p.PicUrl ?? "default.png"
            })
            .ToListAsync();
    }

    // ۴. ساخت خروجی نهایی
    var model = new NewsDetailViewModelDto
    {
        Id = news.Id,
        Title = news.Title,
        CategoryName = news.NC?.Name ?? "-",
        AuthorName = !string.IsNullOrEmpty(news.AuthorName) ? news.AuthorName : "ناشناس",
        IsActive = news.IsActive,
        CreateTime = news.CreateTime ?? DateTime.UtcNow,
        PicUrl = news.PicUrl ?? "default.png",
        RelatedNews = relatedNewsDto,
        Blocks = blocks // لیست بلاک‌ها که حالا پر شده است
    };

    return model;
}

}
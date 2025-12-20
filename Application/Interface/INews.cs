using Application.DTO; 
using Domain;         
using Infra;
using Microsoft.AspNetCore.Hosting;
using Application.Security;
using Microsoft.EntityFrameworkCore;

namespace Application.Interface;

public interface INews
{
    Task<int> CreateNewsAsync(CreatNewsDto dto);
    Task<EditNewsDto> GetForEdit(int id);
    Task Update(EditNewsDto dto);
}

public class NewsService : INews
{
    private readonly AppDbContext _mydb;
    private readonly IWebHostEnvironment _env;
    private readonly IFileSecurityHelper _fileHelper;
    
    public NewsService(AppDbContext mydb, IWebHostEnvironment env,IFileSecurityHelper fileHelper)
    {
        _mydb = mydb;
        _fileHelper = fileHelper;
        _env = env;
    }
    
    public async Task<int> CreateNewsAsync(CreatNewsDto dto)
    {
        // ۱. ساخت آبجکت اولیه (هنوز در دیتابیس ذخیره نمی‌کنیم)
        Console.WriteLine($"[DEBUG-SERVICE] Incoming CategoryId: {dto.NewsCategoryId}");

        var article = new News
        {
            Title = dto.Title,
            AuthorName = dto.AuthorName,
        
            // لاگ منطق تبدیل
            NewsCategoryId = (dto.NewsCategoryId == 0) ? null : dto.NewsCategoryId,

            CreateTime = DateTime.UtcNow, 
            NewsBlocks = new List<NewsBlocks>() 
        };

        // لاگ نهایی قبل از ذخیره
        Console.WriteLine($"[DEBUG-SERVICE] Entity Ready to Save.");
        Console.WriteLine($"[DEBUG-SERVICE] Entity.NewsCategoryId: {(article.NewsCategoryId == null ? "NULL" : article.NewsCategoryId.ToString())}");

        // ۲. مدیریت آپلود عکس
        if (dto.Images != null && dto.Images.Length > 0)
        {
            try 
            {
                var fileName = await _fileHelper.SecureUploadAsync(dto.Images, _env.WebRootPath, "upload/news");
                article.PicUrl = fileName;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Image Upload Failed: {ex.Message}");
            }
        }
    
        // ۳. مدیریت بلوک‌های خبر
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
    
        try 
        {
            await _mydb.News.AddAsync(article);
            await _mydb.SaveChangesAsync();
            Console.WriteLine($"[DEBUG-SERVICE] Save Successful! New ID: {article.Id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DEBUG-SERVICE] CRITICAL ERROR ON SAVE: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"[DEBUG-SERVICE] Inner Exception: {ex.InnerException.Message}");
            }
            throw; // پرتاب مجدد خطا تا برنامه متوجه شود
        }

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
            
            CurrentPicUrl = news.PicUrl, 
            
            Images = null, 

            // 3. تبدیل بلوک‌ها
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
    
            
            if (dto.NewsCategoryId.HasValue)
            {
                news.NewsCategoryId = dto.NewsCategoryId.Value;
            }

            news.UpdatedAt = DateTime.UtcNow; 


            if (dto.Images != null && dto.Images.Length > 0)
            {
                try
                {
                    var fileName = await _fileHelper.SecureUploadAsync(dto.Images, _env.WebRootPath, "upload/news");
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
    
    
}
using System.ComponentModel.DataAnnotations.Schema;
using Domain;
using Microsoft.AspNetCore.Http;

namespace Application.DTO;

public class NewsDetailViewModelDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string AuthorName { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsActive { get; set; }

    public List<NewsBlockViewModelDto> Blocks { get; set; } = new List<NewsBlockViewModelDto>();
    public string? PicUrl { get; set; }
    [NotMapped]
    public IFormFile? pic { get; set; }
    public List<NewsCategorySummaryDto> Categories { get; set; } = new List<NewsCategorySummaryDto>();
    [ForeignKey("NC")]
    public string? CategoryName { get; set; }
    public NewsCategory? NC { get; set; }
    public List<NewsListDto> RelatedNews { get; set; }
    public DateTime CreateTime { get; set; }
    public List<NewsSummaryDto> LatestNews { get; set; } 
}
public class NewsCardDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string CategoryName { get; set; }
    public string? PicUrl { get; set; }
    public bool IsActive { get; set; }
    public bool IsFeatured { get; set; }
    public DateTime CreateTime { get; set; }
    
    //public TYPE  View { get; set; }
}
public class NewsSearchViewModel
{
    public List<NewsCardDto> NewsList { get; set; }
    
    public List<NewsCategorySummaryDto> Categories { get; set; }
    
    public List<int> SelectedCategories { get; set; } = new List<int>();
    public bool IsFeatured { get; set; }
    public bool IsNewest { get; set; }
    
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}



public class NewsBlockViewModelDto
{
    public string Content { get; set; }
    public int BlockType { get; set; } 
    public int SortOrder { get; set; }
}
public class NewsSummaryDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string ImageName { get; set; }
    public string PersianDate { get; set; } // تاریخ شمسی
}
public class NewsCategorySummaryDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int NewsCount { get; set; }
}
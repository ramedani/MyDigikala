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
    [ForeignKey("NC")]
    public string? CategoryName { get; set; }
    public NewsCategory? NC { get; set; }
    public List<NewsListDto> RelatedNews { get; set; }
    public DateTime CreateTime { get; set; }
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
public class NewsBlockViewModelDto
{
    public string Content { get; set; }
    public int BlockType { get; set; } 
    public int SortOrder { get; set; }
}
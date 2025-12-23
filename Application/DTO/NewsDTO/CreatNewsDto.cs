using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Application.DTO;

public class CreatNewsDto
{
    [Required(ErrorMessage = "نام اجباری است")]
    [StringLength(50)]
    public string Title { get; set; }
    public string AuthorName { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsActive { get; set; }
    public int? NewsCategoryId { get; set; }
    public IFormFile? NewsImageMain { get; set; } 
    public List<CreateArticleBlockDto>? NewsBlocks { get; set; }
}

public class NewsImageDto
{
    public int Id { get; set; }
    public string PicUrl { get; set; }
}
public class CreateArticleBlockDto
{
    public string? Content { get; set; } 
    public int? SortOrder { get; set; }  
    public int? BlockType { get; set; }  
}

public class EditNewsDto : CreatNewsDto
{
    public int Id { get; set; }
    public string? CurrentPicUrl { get; set; }
    
}


// نمونه json تولید شده موقع ساخت خبر 
/*
 
{
    "title": "ASP.NET Core Best Practices",
    "authorName": "پارسا صفی لیان",
    "isFeatured": true,
    "isActive": true,
    "newsCategoryId": 3,
    "newsImageMain": null,
    "newsBlocks": [
        {
            "content": "This is the introduction of the article.",
            "sortOrder": 1,
            "blockType": 1
        },
        {
        "content": "Here is the main content section.",
        "sortOrder": 2,
        "blockType": 2
        },
        {
            "content": "Final thoughts and conclusion.",
            "sortOrder": 3,
            "blockType": 1
        }
    ]
}

*/





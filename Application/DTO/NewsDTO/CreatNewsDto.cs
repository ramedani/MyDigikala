using Microsoft.AspNetCore.Http;

namespace Application.DTO;

public class CreatNewsDto
{
    public string Title { get; set; }
    public string AuthorName { get; set; }
    public int? NewsCategoryId { get; set; }
    public IFormFile? Images { get; set; } 
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


// نمونه json تولید شده از این DTO 

// {
//     "Title": "آموزش قهوه",
//     "author": "پارسا سفی لیان",
//     "categoryId": 5,
//     "blocks": [
//         { "sortOrder": 1, "blockType": 1, "content": "مواد لازم" },
//         { "sortOrder": 2, "blockType": 3, "content": "قهوه - شکر - آب" },
//         { "sortOrder": 3, "blockType": 1, "content": "طرز تهیه" },
//         { "sortOrder": 4, "blockType": 2, "content": "آب را جوش بیاورید..." }
//     ]
// }







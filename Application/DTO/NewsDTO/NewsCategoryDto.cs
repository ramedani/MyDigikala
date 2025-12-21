using System.ComponentModel.DataAnnotations;

namespace Application.DTO;

public class NewsCategoryDto
{
    [Required(ErrorMessage = "نام اجباری است")]
    [StringLength(50)]
    public string Name  { get; set; }

}
public class EditNewsCategoryDto : NewsCategoryDto
{
    public int Id { get; set; }
}
public class NewsCategoryListDto
{
    public int Id { get; set; }
    [Required(ErrorMessage = "نام اجباری است")]
    [StringLength(50)]
    public string Name { get; set; }
}


public class DeleteNewsCategoryResultDto
{
    public bool Success { get; set; }
    public bool HasProducts { get; set; }
    public DeleteBlockedNewsCategoryDto BlockInfo { get; set; }
}
public class DeleteBlockedNewsCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<NewsForCategoryBlockDto> News { get; set; }
}

public class NewsForCategoryBlockDto
{
    public int Id { get; set; }
    public string Title { get; set; }

}

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Application.DTO;


    public class CreateCategoryDto
    {
        [Required(ErrorMessage = "نام اجباری است")]
        [StringLength(50)]
        public string Name  { get; set; }
        public IFormFile? Image { get; set; }
    }
    public class EditCategoryDto : CreateCategoryDto
    {
        public int Id { get; set; }
        public string? CurrentImageName { get; set; } 
    }
    public class CategoryListDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? ImageName { get; set; } 
    }



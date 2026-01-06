
using System.ComponentModel.DataAnnotations;

namespace Application.DTO
{
    
    public class CreateBrandDto
    {


        [Required(ErrorMessage = "نام فارسی اجباری است")]
        [StringLength(50)]
        public string NameF { get; set; }
    }
    public class EditBrandDto : CreateBrandDto
    {
        public int Id { get; set; }
    }
    public class BrandListDto
    {
        public int Id { get; set; }

        public string NameF { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class CreateCategoryDTO
    {
        [Required(ErrorMessage ="نام اجباری است")]
        [StringLength(100)]
        public string Name { get; set; }
        [Required(ErrorMessage ="تصویر اجباری است")]
        public string Picture { get; set; }
       
    }
}

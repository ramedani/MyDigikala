using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Application.DTO
{
    public class SiteSettingDTO
    {
        [Display(Name ="نام وب سایت")]
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? KeyWord { get; set; }
        public string? icon { get; set; }
        public string? Logo { get; set; }
        public string? Address { get; set; }
        public string? Tel { get; set; }

        [StringLength(11)]
        public string? Mobile { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public IFormFile?  setlogo{ get; set; }
        public IFormFile? seticon { get; set; }
    }
}

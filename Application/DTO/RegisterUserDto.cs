using Application.Resource;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class RegisterUserDto
    {
        [Required(ErrorMessage = "test")]
        [Display(Name ="test")]
        [MinLength(3, ErrorMessage = "test")]
        public string Username { get; set; }

        [Required(ErrorMessage = "test")]
        [EmailAddress(ErrorMessage = "test")]
        public string Email { get; set; }

        [Required(ErrorMessage = "test")]
        public string Password { get; set; }
    }
}

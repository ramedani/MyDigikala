using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
	public class LoginDTO
	{

        [Required(ErrorMessage = "UserError")]
        [Display(Name = "UserName")]
        public string UserName { get; set; }

		[Required(ErrorMessage = "PassError")]
		[Display(Name = "Pass")]
		public string Password { get; set; }
    }
}

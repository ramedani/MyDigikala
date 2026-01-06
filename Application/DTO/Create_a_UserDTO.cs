using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class Create_a_UserDTO
    {
        [Required(ErrorMessage = "رمز عبور الزامی است")]
        [MinLength(8, ErrorMessage = "رمز عبور حداقل باید ۸ کاراکتر باشد")]
        [MaxLength(64, ErrorMessage = "رمز عبور بیش از حد مجاز است")]
        [DataType(DataType.Password)]
        [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).+$",
        ErrorMessage = "رمز عبور باید شامل حروف بزرگ، کوچک، عدد و کاراکتر خاص باشد"
    )]
        public string Password { get; set; }


        [Required(ErrorMessage = "کد ملی الزامی است")]
        [Range(1000000000, 9999999999, ErrorMessage = "کد ملی باید ۱۰ رقمی باشد")]
        public int MeliCode { get; set; }


        [Required(ErrorMessage = "شماره موبایل الزامی است")]
        [Range(9000000000, 9999999999, ErrorMessage = "شماره موبایل معتبر نیست")]
        public int Phone { get; set; }


        [Required(ErrorMessage = "نام کاربری الزامی است")]
        [MinLength(4, ErrorMessage = "نام کاربری حداقل ۴ کاراکتر")]
        [MaxLength(20, ErrorMessage = "نام کاربری حداکثر ۲۰ کاراکتر")]
        [RegularExpression(
            @"^[a-zA-Z0-9_]+$",
            ErrorMessage = "نام کاربری فقط می‌تواند شامل حروف، عدد و _ باشد"
        )]
        public string Username { get; set; }


        [MaxLength(250, ErrorMessage = "آدرس بیش از حد مجاز است")]
        public string Address { get; set; }


        [Required(ErrorMessage = "ایمیل الزامی است")]
        [EmailAddress(ErrorMessage = "فرمت ایمیل نامعتبر است")]
        [MaxLength(100)]
        public string Email { get; set; }
    }
}

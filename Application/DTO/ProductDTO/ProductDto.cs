using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http; 
namespace Application.DTO
{


    public class ProductListDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="نام اجباری است")]
        [StringLength(100)]
        public string Title { get; set; }
        public string EnTitle { get; set; }
        public decimal Price { get; set; }
        
        public string? Discount { get; set; }
        public float? Invoice { get; set; }
        public bool IsActive { get; set; }
        public string CategoryName { get; set; }
        public string PrimaryImage { get; set; } // عکس اصلی
        public int VisitCount { get; set; }
    }
    public class ProductDashboardStatsDto
    {
        public int TotalProduct { get; set; }
        public int PublishedCount { get; set; }
        public int DraftCount { get; set; }

        //  public string TopNewsTitle { get; set; }
        //  public int TopNewsViews { get; set; }
    }
    public class CreateProductDto
    {
        [Display(Name = "نام محصول (فارسی)")]
        public string Title { get; set; }
        [Display(Name = "نام محصول (انگلیسی)")]
        public string? EnTitle { get; set; } 
        [Display(Name = "توضیحات محصول")]
        public string? Description { get; set; }
        
        public string? Slogan { get; set; }
        [Display(Name = "قیمت محصول")]
        public decimal Price { get; set; }
        [Display(Name = "قیمت خرید محصول")]
        public string? PurchasePrice { get; set; }
        [Display(Name = "قیمت نهایی")]
        public string? Discount { get; set; }
        [Display(Name = "درصد تخفیف محصول")]
        public int DiscountPercentage { get; set; }
        [Display(Name = "برند محصول")]
        public int? BrandId { get; set; }
        [Display(Name = "دسته بندی محصول")]
        public int? CategoryId { get; set; }
        [Display(Name = "کشور محصول")]
        public string? Country { get; set; }
        [Display(Name = "وزن محصول")]
        public string? Weight { get; set; }
        [Display(Name = "موجودی محصول")]
        public float Invoice { get; set; }
        public int? PrimaryImageId { get; set; }
        public bool IsActive { get; set; } = true;
        public bool AmazingOffers { get; set; } = false;
        
        public List<int>? SelectedColorIds { get; set; } = new();
        public List<IFormFile>? NewImages { get; set; }
    }
    
    public class EditProductDto : CreateProductDto
    {
        public int Id { get; set; }

        public List<ProductImageDto> ExistingImages { get; set; } = new();
    }

// کلاس کمکی برای عکس‌ها
    public class ProductImageDto
    {
        public int Id { get; set; }
        public string PicUrl { get; set; }
        public bool IsPrimary { get; set; }
    }

  
}
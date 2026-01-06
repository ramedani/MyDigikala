namespace Application.DTO;

public class ProductDetailViewModelDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string EnTitle { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; } 
    public string? Slogan { get; set; }
    public string? Discount { get; set; }
    public int? DiscountPercentage { get; set; }
    public float Invoice { get; set; }
    public string BrandNameF { get; set; }
    public string BrandNameE { get; set; }
    public string CategoryName { get; set; }
    public bool IsActive { get; set; }
    public int Quantity { get; set; }      // ← جایگزین count
    public string? Country { get; set; }
    public string? Weight { get; set; }

    // تصاویر و رنگ‌ها
    public List<string> Images { get; set; }

    // آمار و ارقام
    public double AverageRating { get; set; }
    public int CommentCount { get; set; }
    public int ApprovedCommentsCount { get; set; }
    
    public int RatedCommentsCount { get; set; } 
    public int QuestionCount { get; set; }
    public bool IsInWishlist { get; set; }


    public List<ProductCommentForViewDto> Comments { get; set; }

    public List<ProductListDto> RelatedProducts { get; set; }


}
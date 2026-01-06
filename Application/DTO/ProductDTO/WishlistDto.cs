namespace Application.DTO;

public class WishlistDto
{
    public int Id { get; set; } // آیدی خود آیتم علاقه‌مندی (برای حذف راحت)
    public int ProductId { get; set; }
    public string Title { get; set; }
    public decimal Price { get; set; }
    public string ImageUrl { get; set; }
    public string Category { get; set; }
}
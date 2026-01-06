namespace Application.DTO;

public class ProductCardDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public decimal Price { get; set; }
    public string CategoryName { get; set; }
    public string ImageUrl { get; set; }
    public DateTime CreateDate { get; set; } 
}
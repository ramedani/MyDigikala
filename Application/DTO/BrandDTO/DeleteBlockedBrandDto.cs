namespace Application.DTO;

public class DeleteBlockedBrandDto
{
    public int Id { get; set; }
    public string NameF { get; set; }
    public List<ProductForBrandBlockDto> Products { get; set; }
}

public class ProductForBrandBlockDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string PrimaryImage { get; set; }
}

namespace Application.DTO;

public class DeleteBlockedCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<ProductForCategoryBlockDto> Products { get; set; }
}

public class ProductForCategoryBlockDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string PrimaryImage { get; set; }
}

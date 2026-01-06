namespace Domain;

public class Brand : BaseTabel
{
    public string Name { get; set; }
    public List<Product>? Products { get; set; }
}
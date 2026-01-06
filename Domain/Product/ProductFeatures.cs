using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;

public class ProductFeatures
{
    public int Id { get; set; }
    
    public string FeatureTitle { get; set; } 

    public string FeatureValue { get; set; } 

    public int SortOrder { get; set; }

    public int BlockType { get; set; } = 1; 

    public int ProductId { get; set; }

    [ForeignKey("ProductId")]
    public Product? Product { get; set; }
}
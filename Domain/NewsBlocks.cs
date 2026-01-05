using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;

public class NewsBlocks
{
    public int Id { get; set; }
    public string? Content { get; set; }
    public int? SortOrder { get; set; }
    public int? BlockType { get; set; } 
    public int? NewsId { get; set; } 
    [ForeignKey("NewsId")]
    public News? News { get; set; } 
}


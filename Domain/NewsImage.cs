using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;

public class NewsImage
{
    public int Id { get; set; }
    public string PicUrl { get; set; }

    [ForeignKey("News")]
    public int NewsId { get; set; }
    public News News { get; set; }
}
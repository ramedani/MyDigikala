namespace Application.DTO;

public class ProductCommentForViewDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public bool IsRecommended { get; set; }
    public string Date { get; set; } 
    public int Like { get; set; }
    public int Dislike { get; set; }
}
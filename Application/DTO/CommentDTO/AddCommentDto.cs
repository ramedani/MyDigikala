namespace Application.DTO;

public class AddCommentDto
{

    public string Name { get; set; }         
    public string Title { get; set; }         
    public bool IsRecommended { get; set; }
    public int ProductId { get; set; }
    public int Rating { get; set; }
    public string Content { get; set; }

    public int? UserId { get; set; } 
}
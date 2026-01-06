namespace Application.DTO;

public class CommentForDetailDto
{
    public int Id { get; set; }
    public string? UserName { get; set; }
    public string Content { get; set; }

    public int? Like { get; set; }        // ✅
    public int? Dislike { get; set; }



   // public string Date { get; set; } 
}
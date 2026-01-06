namespace Application.DTO;

public class CommentDto
{
    public int Id { get; set; }
    public string Content { get; set; }
    public string Title { get; set; }
    public bool IsConfirmed { get; set; }
    public string ProductName { get; set; }
    public int ProductId { get; set; }
    public string? UserName { get; set; }

}
public class CommentListDto
{
    public int Id { get; set; }
    public string Content { get; set; }
    public string ProductName { get; set; } 
    public string? UserName { get; set; }
    public bool IsSelectedForHome { get; set; }
}


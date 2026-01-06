namespace Application.DTO;

public class AdminCommentsViewModel
{
    public List<CommentDto> Pending { get; set; }
    public List<CommentDto> Approved { get; set; }
}
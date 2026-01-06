using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;

public class CommentVote : BaseTabel
{
    
    public int CommentId { get; set; }
    [ForeignKey("CommentId")]
    public virtual ProductComment Comment { get; set; }
    /*
    public int UserId { get; set; }
    [ForeignKey("UserId")]
    public virtual User User { get; set; } 
    */
    public bool IsLike { get; set; } 
}
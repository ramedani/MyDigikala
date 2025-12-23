namespace Application.DTO;

public class NewsListDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string AuthorName { get; set; }
    public string CategoryName { get; set; }
    public string? PicUrl { get; set; }
    public bool IsActive { get; set; }
    public bool IsFeatured { get; set; }
    public DateTime CreateTime { get; set; }
    

}

public class NewsDashboardStatsDto
{
    public int TotalNews { get; set; }
    public int PublishedCount { get; set; }
    public int DraftCount { get; set; }
    // برای ویجت "پربازدیدترین"
  //  public string TopNewsTitle { get; set; }
  //  public int TopNewsViews { get; set; }
}
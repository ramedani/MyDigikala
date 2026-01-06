namespace Application.DTO;

public class DeleteCategoryResultDto
{
    public bool Success { get; set; }
    public bool HasProducts { get; set; }
    public DeleteBlockedCategoryDto BlockInfo { get; set; }
}
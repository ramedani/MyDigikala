namespace Application.DTO;

public class DeleteBrandResultDto
{
    public bool Success { get; set; }
    public bool HasProducts { get; set; }
    public DeleteBlockedBrandDto BlockInfo { get; set; }
}
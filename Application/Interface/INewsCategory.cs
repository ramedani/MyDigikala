using Application.DTO;
using Domain;
using Infra;
using Microsoft.EntityFrameworkCore;



namespace Application.Interface;

public interface INewsCategory
{
    Task<int> Create(NewsCategoryDto dto);
    Task<EditNewsCategoryDto> Get(int id);
    Task<List<NewsCategoryListDto>> GetAll();
    Task Update(EditNewsCategoryDto dto);
    Task<DeleteNewsCategoryResultDto> Delete(int id);
}

public class NewsCategoryService : INewsCategory
{
    private AppDbContext mydb;

    public NewsCategoryService(AppDbContext _mydb)
    {
        mydb = _mydb;
    }

    public async Task<int> Create(NewsCategoryDto dto)
    {
        var category = new NewsCategory
        {
            Name = dto.Name
        };

        mydb.NewsCategories.Add(category);

        await mydb.SaveChangesAsync();
        return category.Id;
    }

    public async Task<EditNewsCategoryDto> Get(int id)
    {
        var category = await mydb.NewsCategories.FindAsync(id);
        if (category == null) return null;

        return new EditNewsCategoryDto
        {
            Id = category.Id,
            Name = category.Name
        };
    }

    public async Task<List<NewsCategoryListDto>> GetAll()
    {
        return await mydb.NewsCategories
            .Select(b => new NewsCategoryListDto
            {
                Id = b.Id,
                Name = b.Name
            }).ToListAsync();
    }

    public async Task Update(EditNewsCategoryDto dto)
    {
        var category = await mydb.NewsCategories.FindAsync(dto.Id);
    
        if (category == null)
        {
            throw new KeyNotFoundException($"Category with ID {dto.Id} not found");
        }

        category.Name = dto.Name;


        await mydb.SaveChangesAsync();
    }
    
    public async Task<DeleteNewsCategoryResultDto> Delete(int id)
    {
        var category = await mydb.NewsCategories
            .Include(c => c.News) 
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
            return new DeleteNewsCategoryResultDto { Success = false };

        if (category.News != null && category.News.Any())
        {
            return new DeleteNewsCategoryResultDto
            {
                Success = false,
                HasProducts = true,
                BlockInfo = new DeleteBlockedNewsCategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    News = category.News.Select(p => new NewsForCategoryBlockDto
                    {
                        Id = p.Id,
                        Title = p.Title
                    }).ToList()
                }
            };
        }

        mydb.NewsCategories.Remove(category);
        await mydb.SaveChangesAsync();

        return new DeleteNewsCategoryResultDto
        {
            Success = true,
            HasProducts = false
        };
    }
}
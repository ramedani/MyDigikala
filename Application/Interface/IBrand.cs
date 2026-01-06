using Application.DTO;
using Domain;
using Infra;
using Microsoft.EntityFrameworkCore;

namespace Application.Interface
{
    public interface IBrand
    {
        Task<int> Create(CreateBrandDto dto);
        Task<EditBrandDto> Get(int id);
        Task<List<BrandListDto>> GetAll();
        Task Update(EditBrandDto dto);
        Task<DeleteBrandResultDto> Delete(int id);

    }

    public class BrandService : IBrand
    {
        private AppDbContext mydb;

        public BrandService(AppDbContext _mydb)
        {
            mydb = _mydb;
        }

        public async Task<int> Create(CreateBrandDto dto)
        {
            var brand = new Brand
            {
                Name = dto.NameF
            };

            mydb.Brands.Add(brand);
            await mydb.SaveChangesAsync();
            return brand.Id;
        }

        public async Task<EditBrandDto> Get(int id)
        {
            var brand = await mydb.Brands.FindAsync(id);
            if (brand == null) return null;

            return new EditBrandDto
            {
                Id = brand.Id,
                NameF = brand.Name
            };
        }

        public async Task<List<BrandListDto>> GetAll()
        {
            return await mydb.Brands
                .Select(b => new BrandListDto
                {
                    Id = b.Id,
                    NameF = b.Name
                }).ToListAsync();
        }

        public async Task Update(EditBrandDto dto)
        {
            var brand = await mydb.Brands.FindAsync(dto.Id);
            if (brand == null) return;

            brand.Name = dto.NameF;


            await mydb.SaveChangesAsync();
        }
        
        public async Task<DeleteBrandResultDto> Delete(int id)
        {
            var brand = await mydb.Brands
                .Include(b => b.Products)
                .ThenInclude(p => p.images)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (brand == null)
                return new DeleteBrandResultDto { Success = false };

            if (brand.Products != null && brand.Products.Any())
            {
                return new DeleteBrandResultDto
                {
                    Success = false,
                    HasProducts = true,
                    BlockInfo = new DeleteBlockedBrandDto
                    {
                        Id = brand.Id,
                        NameF = brand.Name,
                        Products = brand.Products.Select(p => new ProductForBrandBlockDto
                        {
                            Id = p.Id,
                            Title = p.Title,
                            PrimaryImage = p.images?.FirstOrDefault(i => i.IsPrimary)?.PicUrl
                        }).ToList()
                    }
                };
            }

            mydb.Brands.Remove(brand);
            await mydb.SaveChangesAsync();

            return new DeleteBrandResultDto
            {
                Success = true,
                HasProducts = false
            };
        }
    }
}
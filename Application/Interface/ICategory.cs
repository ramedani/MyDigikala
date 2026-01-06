using Application.DTO;
using Application.Helper;
using Domain;
using Infra;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace Application.Interface{
    public interface ICategory
    {
        
             Task<int> Create(CreateCategoryDto dto);
            Task<EditCategoryDto> Get(int id);
            Task<List<CategoryListDto>> GetAll();
            Task Update(EditCategoryDto dto);
            Task<DeleteCategoryResultDto> Delete(int id);

    }

    public class CategoryService : ICategory
    {
        private AppDbContext mydb;
        private readonly IWebHostEnvironment _env;
        private readonly IFileSecurityHelper _fileHelper;

         public CategoryService(AppDbContext _mydb,IWebHostEnvironment env,IFileSecurityHelper fileHelper)
            {
                mydb = _mydb;
                _env = env;
                _fileHelper = fileHelper;
            }

            public async Task<int> Create(CreateCategoryDto dto)
            {
                var category = new Category
                {
                    Name = dto.Name
                };
                if (dto.Image != null)
                {
                    try
                    {
                        // عکس‌ها توی پوشه wwwroot/CategoryImages ذخیره میشن
                        var imagePath = await _fileHelper.SecureUploadAsync(dto.Image, _env.WebRootPath, "CategoryImages");
                    
                        // هلپر مسیر کامل نسبی رو برمی‌گردونه (مثلاً /CategoryImages/guid.jpg)
                        // اما چون ممکنه بخوای فقط اسم فایل رو داشته باشی، اینجا یکم تمیزکاری می‌کنیم:
                        // البته اگر هلپر شما مسیر کامل (/Folder/File.jpg) برمیگردونه، بهتره همون رو ذخیره کنیم که تو ویو راحت باشیم.
                        category.PicUrl = Path.GetFileName(imagePath); 
                    }
                    catch (Exception ex)
                    {
                        // اینجا میتونی لاگ بزنی
                        // اگر آپلود فیل شد، فعلا عکس نال میمونه
                        Console.WriteLine($"Upload Failed: {ex.Message}");
                    }
                }

                mydb.Categories.Add(category);
                
                await mydb.SaveChangesAsync();
                return category.Id;
            }

         public async Task<EditCategoryDto> Get(int id)
        {
            var category = await mydb.Categories.FindAsync(id);
            if (category == null) return null;

            return new EditCategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                CurrentImageName = category.PicUrl
            };
        }

        public async Task<List<CategoryListDto>> GetAll()
        {
            return await mydb.Categories
                .Select(b => new CategoryListDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    ImageName = b.PicUrl 
                }).ToListAsync();
        }

        public async Task Update(EditCategoryDto dto)
        {
            var category = await mydb.Categories.FindAsync(dto.Id);
            if (category == null)
                throw new KeyNotFoundException($"Category with ID {dto.Id} not found");

            category.Name = dto.Name;

            // آپلود عکس جدید
            if (dto.Image != null)
            {
                try
                {
                    // 1. حذف عکس قدیمی اگر وجود داره
                    if (!string.IsNullOrEmpty(category.PicUrl))
                    {
                        var oldPath = Path.Combine(_env.WebRootPath, "CategoryImages", category.PicUrl);
                        if (System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                        }
                    }

                    // 2. آپلود عکس جدید با هلپر
                    var newImagePath = await _fileHelper.SecureUploadAsync(dto.Image, _env.WebRootPath, "CategoryImages");
                    category.PicUrl = Path.GetFileName(newImagePath);
                }
                catch (Exception ex)
                {
                    // مدیریت خطا
                }
            }

            await mydb.SaveChangesAsync();
        }

        public async Task<DeleteCategoryResultDto> Delete(int id)
        {
            var category = await mydb.Categories
                .Include(b => b.Products)
                .ThenInclude(p => p.images)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (category == null)
                return new DeleteCategoryResultDto { Success = false };

            // چک کردن اینکه آیا محصولی دارد یا خیر (منطق بیزنس)
            if (category.Products != null && category.Products.Any())
            {
                return new DeleteCategoryResultDto
                {
                    Success = false,
                    HasProducts = true,
                    BlockInfo = new DeleteBlockedCategoryDto
                    {
                        Id = category.Id,
                        Name = category.Name,
                        Products = category.Products.Select(p => new ProductForCategoryBlockDto
                        {
                            Id = p.Id,
                            Title = p.Title,
                            PrimaryImage = p.images?.FirstOrDefault(i => i.IsPrimary)?.PicUrl
                        }).ToList()
                    }
                };
            }

            // حذف فیزیکی عکس دسته‌بندی از سرور
            if (!string.IsNullOrEmpty(category.PicUrl))
            {
                var path = Path.Combine(_env.WebRootPath, "CategoryImages", category.PicUrl);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }

            mydb.Categories.Remove(category);
            await mydb.SaveChangesAsync();

            return new DeleteCategoryResultDto
            {
                Success = true,
                HasProducts = false
            };
        }
        }
   
}
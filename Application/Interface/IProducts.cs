using Application.DTO; 
using Domain;         
using Infra;          
using Application.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Interface
{
 

    public interface IProducts
    {
 
        Task<int> CreateProductAsync(CreateProductsDTO dto);
        Task<EditProductDto> GetForEdit(int id);
        Task Update(EditProductDto dto);
        Task<List<ProductListDto>> GetAll();
        Task Delete(int id);
    }

    public class ProductService : IProducts
    {
        private readonly AppDbContext mydb;
        private readonly IWebHostEnvironment _env;
        private readonly IFileSecurityHelper _fileHelper;
        public ProductService(AppDbContext _mydb, IWebHostEnvironment env,IFileSecurityHelper fileHelper)
        {
            mydb = _mydb;
            _env = env;
            _fileHelper = fileHelper;
        }
        public async Task<List<ProductListDto>> GetAll()
        {
            return await mydb.Products
                .OrderByDescending(p => p.Id)
                .Select(p => new ProductListDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Price = p.Price,
       
                })
                .ToListAsync();
        }

        // نام متد را با اینترفیس هماهنگ کردیم
        public async Task<int> CreateProductAsync(CreateProductsDTO dto)
        {
            // 1. مپ کردن اطلاعات ساده
            var product = new Product
            {
                Title = dto.Title,
                Description = dto.Description,
                Price = dto.Price,
                Invoice =  dto.Invoice
                // IsActive = true, // اگر فیلد فعال بودن دارید
                // CreateDate = DateTime.Now // اگر BaseEntity ندارید
            };

            // 2. ذخیره اولیه برای گرفتن ID
            mydb.Products.Add(product);
            await mydb.SaveChangesAsync();

            // 3. آپلود و ذخیره عکس‌ها
            if (dto.images != null && dto.images.Count > 0)
            {
                foreach (var file in dto.images)
                {
                    if (file != null && file.Length > 0)
                    {
                        try
                        {
                            // آپلود امن فایل
                            var fileName = await _fileHelper.SecureUploadAsync(file, _env.WebRootPath, "upload");

                            // اولین عکس به عنوان عکس اصلی؟ (اختیاری)
                            // bool isFirst = !mydb.ProductImages.Any(x => x.ProductId == product.Id);

                            mydb.ProductImages.Add(new ProductImage
                            {
                                ProductId = product.Id,
                                PicUrl = fileName
                            });
                        }
                        catch (Exception ex)
                        {
                            // لاگ کردن خطا - فعلا نادیده می‌گیریم تا پروسه متوقف نشود
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
                // ذخیره نهایی عکس‌ها
                await mydb.SaveChangesAsync();
            }

            return product.Id;
        }


        public async Task<EditProductDto> GetForEdit(int id)
        {
            var p = await mydb.Products
                .Include(x => x.images)
             
                .FirstOrDefaultAsync(x => x.Id == id);

            if (p == null) return null;

            return new EditProductDto
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                Invoice =  p.Invoice,
                Price = p.Price,
          
                // تبدیل رنگ‌های موجود به لیست ID
              
                // تبدیل عکس‌ها برای نمایش
                ExistingImages = p.images?.Select(i => new ProductImageDto
                {
                    Id = i.Id,
                    PicUrl = i.PicUrl
           
                }).ToList() ?? new List<ProductImageDto>(),

          
            };
        }

        public async Task Update(EditProductDto dto)
        {
            var product = await mydb.Products
     
                .Include(p => p.images)
                .FirstOrDefaultAsync(p => p.Id == dto.Id);

            if (product == null) return;

            // 1. آپدیت فیلدها
            product.Title = dto.Title;
            product.Description = dto.Description;
            product.Invoice = dto.Invoice;
            product.Price = dto.Price;
  

            // 2. آپدیت رنگ‌ها (حذف قبلی‌ها و افزودن جدیدها)
           

            // 3. افزودن عکس‌های جدید
            if (dto.images != null && dto.images.Count > 0)
            {
                foreach (var file in dto.images)
                {
                    try
                    {
                        var fileName = await _fileHelper.SecureUploadAsync(file, _env.WebRootPath, "upload");
                        mydb.ProductImages.Add(new ProductImage
                        {
                            ProductId = product.Id,
                            PicUrl = fileName
                        });
                    }
                    catch (Exception ex)
                    {
                        // لاگ کردن خطا
                    }
                }
            }

            // ذخیره تغییرات تا اینجا (مخصوصا عکس‌های جدید که باید ID بگیرن)
            await mydb.SaveChangesAsync();

            // 4. مدیریت عکس اصلی (Primary Image)
           
        }
        public async Task Delete(int id)
        {
            var product = await mydb.Products.FindAsync(id);
            if (product != null)
            {
                // عکس‌ها رو هم پاک کنیم؟ یا فقط رکورد؟
                // فعلا ساده پاک می‌کنیم. EF خودش Cascade Delete می‌کنه معمولا.
                mydb.Products.Remove(product);
                await mydb.SaveChangesAsync();
            }
        }













        
    }
}

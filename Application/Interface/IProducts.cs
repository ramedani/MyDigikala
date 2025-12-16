using Application.DTO; // مطمئن شوید این namespace درست است
using Domain;         // مطمئن شوید این namespace درست است
using Infra;          // اگر کلاسی از اینجا استفاده می‌کنید

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
        // نام متد را یکسان کردیم
        Task<int> CreateProductAsync(CreateProductsDTO dto);
        Task<CreateProductsDTO> GetForEdit(int id);
        Task Update(CreateProductsDTO dto);
    }

    public class ProductService : IProducts
    {
        private readonly AppDbContext mydb;
        private readonly IWebHostEnvironment _env;

        public ProductService(AppDbContext _mydb, IWebHostEnvironment env)
        {
            mydb = _mydb;
            _env = env;
        }

        // نام متد را با اینترفیس هماهنگ کردیم
        public async Task<int> CreateProductAsync(CreateProductsDTO dto)
        {
            // 1. مپ کردن اطلاعات ساده
            var product = new Product
            {
                Title = dto.Title,
                Description = dto.Description,
                Price = dto.Price
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
                            var fileName = await SecureUploadFileAsync(file);

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


        public async Task<CreateProductsDTO> GetForEdit(int id)
        {
            var p = await mydb.Products
                .Include(x => x.images)
             
                .FirstOrDefaultAsync(x => x.Id == id);

            if (p == null) return null;

            return new CreateProductsDTO
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
           
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

        public async Task Update(CreateProductsDTO dto)
        {
            var product = await mydb.Products
     
                .Include(p => p.images)
                .FirstOrDefaultAsync(p => p.Id == dto.Id);

            if (product == null) return;

            // 1. آپدیت فیلدها
            product.Title = dto.Title;
            product.Description = dto.Description;

            product.Price = dto.Price;
  

            // 2. آپدیت رنگ‌ها (حذف قبلی‌ها و افزودن جدیدها)
           

            // 3. افزودن عکس‌های جدید
            if (dto.images != null && dto.images.Count > 0)
            {
                foreach (var file in dto.images)
                {
                    try
                    {
                        var fileName = await SecureUploadFileAsync(file);
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
















        // متد خصوصی برای آپلود داخل همین سرویس
        private async Task<string> SecureUploadFileAsync(IFormFile file)
        {
            // 1. اعتبارسنجی فایل
            var validationResult = await FileUploadHelper.ValidateFileAsync(file);
            if (!validationResult.IsValid)
            {
                throw new ArgumentException(validationResult.ErrorMessage);
            }

            // 2. ساخت نام یکتا
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var fileName = $"{Guid.NewGuid()}{fileExtension}";

            // مسیر ذخیره‌سازی: wwwroot/upload
            var uploadPath = Path.Combine(_env.WebRootPath, "upload");

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            var filePath = Path.Combine(uploadPath, fileName);

            // 3. ذخیره فایل
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // برگرداندن مسیر نسبی (برای نمایش در HTML)
            return $"/upload/{fileName}";
        }
    }

    // کلاس کمکی استاتیک (بهتر است در لایه Infra باشد اما اینجا هم کار می‌کند)
    public static class FileUploadHelper
    {
        private static readonly Dictionary<string, List<byte[]>> _fileSignature =
            new Dictionary<string, List<byte[]>>
            {
                { ".png", new List<byte[]> { new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } } },
                { ".jpeg", new List<byte[]>
                    {
                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 },
                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE8 }
                    }
                },
                { ".jpg", new List<byte[]>
                    {
                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 },
                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE8 }
                    }
                },
                { ".gif", new List<byte[]> { new byte[] { 0x47, 0x49, 0x46, 0x38 } } },
                { ".webp", new List<byte[]>
                    {
                        new byte[] { 0x52, 0x49, 0x46, 0x46 }, // RIFF
                    }
                }
            };

        private static readonly string[] _allowedExtensions = { ".png", ".jpeg", ".jpg", ".gif", ".webp" };
        private static readonly string[] _allowedMimeTypes = { "image/jpeg", "image/png", "image/gif", "image/webp" };
        private const int MaxFileSizeInBytes = 5 * 1024 * 1024; // 5 MB

        public static async Task<FileUploadValidationResultDto> ValidateFileAsync(IFormFile file)
        {
            // اصلاح ارور 1: عملگر || اضافه شد
            if (file == null || file.Length == 0)
            {
                return new FileUploadValidationResultDto { IsValid = false, ErrorMessage = "فایل خالی است." };
            }

            if (file.Length > MaxFileSizeInBytes)
            {
                return new FileUploadValidationResultDto
                {
                    IsValid = false,
                    ErrorMessage = $"حجم فایل نباید بیشتر از 5 مگابایت باشد."
                };
            }

            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

            // اصلاح ارور 2: عملگر || اضافه شد
            if (string.IsNullOrEmpty(fileExtension) || !_allowedExtensions.Contains(fileExtension))
            {
                return new FileUploadValidationResultDto
                {
                    IsValid = false,
                    ErrorMessage = "فرمت فایل مجاز نیست."
                };
            }

            if (!_allowedMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
            {
                return new FileUploadValidationResultDto
                { IsValid = false, ErrorMessage = "نوع محتوا (MIME) معتبر نیست." };
            }

            // بررسی Magic Bytes
            using (var stream = file.OpenReadStream())
            using (var reader = new BinaryReader(stream))
            {
                if (!_fileSignature.ContainsKey(fileExtension))
                {
                    return new FileUploadValidationResultDto { IsValid = false, ErrorMessage = "فرمت ناشناخته." };
                }

                var signatures = _fileSignature[fileExtension];
                var headerBytes = reader.ReadBytes(signatures.Max(m => m.Length));

                bool isSignatureValid = signatures.Any(signature =>
                    headerBytes.Take(signature.Length).SequenceEqual(signature));

                // منطق WebP
                if (fileExtension == ".webp" && isSignatureValid)
                {
                    if (file.Length < 12)
                    {
                        isSignatureValid = false;
                    }
                    else
                    {
                        // باید استریم را ریست کنیم یا با دقت بخوانیم
                        // چون reader قبلا چند بایت خوانده، موقعیت تغییر کرده.
                        // برای سادگی اینجا دوباره بافر را پر می‌کنیم از اول
                        stream.Position = 0;
                        byte[] buffer = new byte[12];
                        stream.Read(buffer, 0, 12);

                        // چک کردن WEBP
                        isSignatureValid = buffer[8] == 0x57 && buffer[9] == 0x45 && buffer[10] == 0x42 && buffer[11] == 0x50;
                    }
                }

                if (!isSignatureValid)
                {
                    return new FileUploadValidationResultDto
                    { IsValid = false, ErrorMessage = "فایل معتبر نیست (Signature mismatch)." };
                }
            }

            return new FileUploadValidationResultDto { IsValid = true };
        }
    }
}

using Application.DTO; 
using Domain;         
using Infra;          

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;

namespace Application.Security;

  public interface IFileSecurityHelper
    {
        Task<FileUploadValidationResultDto> ValidateFileAsync(IFormFile file);
        Task<string> SecureUploadAsync(IFormFile file, string webRootPath, string folderName);
    }

    public class FileSecurityHelper : IFileSecurityHelper
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
                { ".webp", new List<byte[]> { new byte[] { 0x52, 0x49, 0x46, 0x46 } } }
            };

        private static readonly string[] _allowedExtensions = { ".png", ".jpeg", ".jpg", ".gif", ".webp" };
        private static readonly string[] _allowedMimeTypes = { "image/jpeg", "image/png", "image/gif", "image/webp" };
        private const int MaxFileSizeInBytes = 5 * 1024 * 1024; // 5 MB

        public async Task<FileUploadValidationResultDto> ValidateFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return new FileUploadValidationResultDto { IsValid = false, ErrorMessage = "فایل خالی است." };

            if (file.Length > MaxFileSizeInBytes)
                return new FileUploadValidationResultDto { IsValid = false, ErrorMessage = "حجم فایل نباید بیشتر از 5 مگابایت باشد." };

            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(fileExtension) || !_allowedExtensions.Contains(fileExtension))
                return new FileUploadValidationResultDto { IsValid = false, ErrorMessage = "فرمت فایل مجاز نیست." };

            if (!_allowedMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
                return new FileUploadValidationResultDto { IsValid = false, ErrorMessage = "نوع محتوا (MIME) معتبر نیست." };

            // بررسی Magic Bytes
            using (var stream = file.OpenReadStream())
            using (var reader = new BinaryReader(stream))
            {
                if (!_fileSignature.ContainsKey(fileExtension))
                    return new FileUploadValidationResultDto { IsValid = false, ErrorMessage = "فرمت ناشناخته." };

                var signatures = _fileSignature[fileExtension];
                var headerBytes = reader.ReadBytes(signatures.Max(m => m.Length));

                bool isSignatureValid = signatures.Any(signature =>
                    headerBytes.Take(signature.Length).SequenceEqual(signature));

                if (fileExtension == ".webp" && isSignatureValid)
                {
                    if (file.Length < 12) isSignatureValid = false;
                    else
                    {
                        stream.Position = 0;
                        byte[] buffer = new byte[12];
                        stream.Read(buffer, 0, 12);
                        isSignatureValid = buffer[8] == 0x57 && buffer[9] == 0x45 && buffer[10] == 0x42 && buffer[11] == 0x50;
                    }
                }

                if (!isSignatureValid)
                    return new FileUploadValidationResultDto { IsValid = false, ErrorMessage = "فایل معتبر نیست (Signature mismatch)." };
            }

            return new FileUploadValidationResultDto { IsValid = true };
        }

        public async Task<string> SecureUploadAsync(IFormFile file, string webRootPath, string folderName)
        {
            // اول اعتبارسنجی
            var validation = await ValidateFileAsync(file);
            if (!validation.IsValid)
                throw new Exception(validation.ErrorMessage);

            // دوم آپلود
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var uploadPath = Path.Combine(webRootPath, folderName);

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/{folderName}/{fileName}";
        }
    }
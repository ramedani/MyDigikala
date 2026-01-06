using Application.DTO;
using Domain;
using Infra;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;



namespace Application.Interface;

public interface IWishlist
{
    Task<List<WishlistDto>> GetUserWishlistAsync(string userId);
    Task<OperationResult> ToggleWishlistItemAsync(string userId, int productId);
    Task<OperationResult> RemoveItemAsync(string userId, int productId);
    Task<bool> IsProductInWishlistAsync(string userId, long productId);
}
    
// یک کلاس ساده برای مدیریت نتیجه عملیات
public class OperationResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public string Status { get; set; } // مثل "added" یا "removed"
}
 public class WishlistService : IWishlist
    {
        private readonly AppDbContext _context;

        public WishlistService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<WishlistDto>> GetUserWishlistAsync(string userId)
        {
            // استفاده از Select باعث می شود فقط فیلد های مورد نیاز از دیتابیس واکشی شوند
            // این کار پرفورمنس را بسیار بالا می برد
            return await _context.WishlistItems
                .Where(w => w.UserId == userId)
                // نکته مهم: اینجا نیازی به Include نیست چون در Select مستقیماً دسترسی داریم
                // اما اگر Select نمی‌زدیم، Include ضروری بود.
                .Select(w => new WishlistDto
                {
                    Id = w.ProductId, // برای حذف راحت
                    ProductId = w.ProductId,
                    Title = w.Product.Title,

                    Price = w.Product.Price, 
            
                    // *** بخش مهم اصلاح شده برای عکس ***
                    // منطق: برو تو لیست عکسها، اونی که IsPrimary هست رو پیدا کن
                    // اگر نبود، اولین عکس رو بده. اگر کلا عکس نداشت، "no-image.jpg"
                    ImageUrl = w.Product.images.FirstOrDefault(i => i.IsPrimary).PicUrl 
                               ?? w.Product.images.FirstOrDefault().PicUrl 
                               ?? "no-image.jpg",
                       
                    Category = w.Product.cat.Name
                    // فرض بر این که مدل Category فیلد Title دارد
                })
                .ToListAsync();
        }

        public async Task<OperationResult> ToggleWishlistItemAsync(string userId, int productId)
        {
            var existingItem = await _context.WishlistItems
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

            if (existingItem != null)
            {
                _context.WishlistItems.Remove(existingItem);
                await _context.SaveChangesAsync();
                return new OperationResult { IsSuccess = true, Message = "از لیست حذف شد", Status = "removed" };
            }

            var newItem = new WishlistItem
            {
                UserId = userId,
                ProductId = productId
            };

            _context.WishlistItems.Add(newItem);
            await _context.SaveChangesAsync();
            return new OperationResult { IsSuccess = true, Message = "به لیست اضافه شد", Status = "added" };
        }

        public async Task<OperationResult> RemoveItemAsync(string userId, int productId)
        {
            var item = await _context.WishlistItems
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

            if (item == null)
                return new OperationResult { IsSuccess = false, Message = "یافت نشد" };

            _context.WishlistItems.Remove(item);
            await _context.SaveChangesAsync();
            return new OperationResult { IsSuccess = true, Message = "حذف شد" };
        }
        public async Task<bool> IsProductInWishlistAsync(string userId, long productId)
        {
            return await _context.WishlistItems
                .AnyAsync(w => w.UserId == userId && w.ProductId == productId);
        }
    }
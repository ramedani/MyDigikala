using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;

public class WishlistItem
{
    [Key]
    public int Id { get; set; }

    // آیدی کاربری که لایک کرده
    [Required]
    public string UserId { get; set; } 
    // نکته: اگر آیدی کاربر شما int است، نوع این را به int تغییر دهید.
    // در ASP.NET Identity پیش‌فرض string است.

    // آیدی محصولی که لایک شده
    public int ProductId { get; set; }

    // ارتباط با جدول محصولات (برای گرفتن عکس و قیمت و...)
    [ForeignKey("ProductId")]
    public virtual Product Product { get; set; }
        
    // تاریخ اضافه شدن (اختیاری ولی مفید برای آنالیز)
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
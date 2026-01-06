using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
   public class Product : BaseTabel
    {
        public string Title { get; set; }
        public string? EnTitle { get; set; } 
        public string? Description { get; set; }
        public string? Slogan { get; set; }//شعار تبلیفاتی
        public string? PurchasePrice { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public string? Discount { get; set; }
        public int DiscountPercentage { get; set; }
        [ForeignKey("brand")]
        public int? BrandId { get; set; }
        public Brand? brand { get; set; }
        public string? Country { get; set; }
        public string? Weight { get; set; }
        public List<ProductImage>? images { get; set; }
        [ForeignKey("cat")]
        public int? CategoryId { get; set; }
        public Category? cat { get; set; }
        public ICollection<ProductFeatures>? ProductFeatures { get; set; } 
        public float Invoice { get; set; }
        public bool AmazingOffers { get; set; } = false;
        public int? count { get; set; } = 1;
        [NotMapped]
        public int? PrimaryImageId { get; set; }

        public List<ProductComment>? Msg4Products { get; set; }
        public bool IsActive { get; set; } = true;
        public int VisitCount { get; set; } = 0;
        public DateTime CreateDate { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ProductImage : BaseTabel
    {
        public string PicUrl { get; set; }
        public bool IsPrimary { get; set; }
        [ForeignKey("prd")]
        public int ProductId { get; set; }
        public Product prd { get; set; }
    }
}

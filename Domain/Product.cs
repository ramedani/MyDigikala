using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
   public class Product
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public float Invoice { get; set; }
        public List<ProductComment>? prdcmt { get; set; }

        public List<ProductImage>? images { get; set; }


    }
}

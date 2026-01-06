using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Category:BaseTabel
    {
        public string Name { get; set; }
        public string? PicUrl { get; set; }
        public List<Product>? Products { get; set; }
        
    }
}

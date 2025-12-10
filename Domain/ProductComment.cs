using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ProductComment
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Comment { get; set; }
        public bool Suggestion { get; set; }
        public string Username { get; set; }
        public DateTime AddComment { get; set; }
        [ForeignKey("prd")]
        public int ProductId { get; set; }
        public Product prd { get; set; }
    }
}

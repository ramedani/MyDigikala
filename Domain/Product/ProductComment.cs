using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ProductComment : BaseTabel
    {
        public string? Name { get; set; }
        public string? Title { get; set; }
        public bool? IsRecommended { get; set; }
        public string? Content { get; set; }
        public int? FiveStar { get; set; } = 0;
        public string? Strengths { get; set; }
        public string? Weaknesses { get; set; }
        public int? Like { get; set; } 
        public int? Dislike { get; set; } 
        [ForeignKey("prdmsg")]
        public int ProductId { get; set; }
        public Product prdmsg { get; set; }
        public bool IsConfirmed { get; set; } = false;

    }
}

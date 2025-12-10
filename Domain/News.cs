using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class News
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content  { get; set; }

        [ForeignKey("NC")]
        public int NewsCategoryId { get; set; }
        public NewsCategory NC { get; set; }
    }
}

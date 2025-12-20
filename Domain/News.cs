using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Domain 
{
    public class News : BaseTabel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string AuthorName { get; set; }
        
        public ICollection<NewsBlocks>? NewsBlocks { get; set; } 

        public string? PicUrl { get; set; }
        [NotMapped]
        public IFormFile? pic { get; set; }
        [ForeignKey("NC")]
        public int? NewsCategoryId { get; set; }
        public NewsCategory? NC { get; set; }
    }
}

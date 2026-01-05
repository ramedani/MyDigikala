using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class BaseTabel
    {
        [Key]
        public int Id { get; set; }

        public string? UserInsert { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

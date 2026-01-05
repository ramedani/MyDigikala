using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class SiteSetting:BaseTabel
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? KeyWord { get; set; }
        public string? icon{ get; set; }
        public string? Logo { get; set; }
        public string? Address { get; set; }
        public string? Tel { get; set; }
        public string? Mobile { get; set; }
        public string? Email { get; set; }

    }
}

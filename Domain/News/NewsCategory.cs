using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class NewsCategory : BaseTabel
    {

        public string? Name { get; set; }

        public List<News>? News { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class register:BaseTabel
    {
    

        public string Password { get; set; }
        public int MeliCode { get; set; }

        public int Phone  { get; set; }


        public string username { get; set; }

        public string Address { get; set; }

        public string Email { get; set; }
    
    }
}

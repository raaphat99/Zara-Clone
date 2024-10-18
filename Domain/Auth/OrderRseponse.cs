using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Auth
{
    public class OrderResponse
    {
        public string Status { get; set; }
        public string RedirectUrl { get; set; }
    }
}

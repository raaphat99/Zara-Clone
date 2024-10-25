using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum OrderStatus
    {
        Pending = 1,
        Shipped = 2,
        Delivered = 3,
        Canceled = 4
    }
}

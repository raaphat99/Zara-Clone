using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class ShippingMethod
    {
        public int Id { get; set; }
        public ShippingName Name { get; set; }
        public double ShippingCost { get; set; }
        public int EstimatedDeliveryTime { get; set; }
        public virtual ICollection<Order> Orders { get; set; }=new HashSet<Order>();

        public enum ShippingName
        {
            Standard,
            Free,
            
        }
    }
}

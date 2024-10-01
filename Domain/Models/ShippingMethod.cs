using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class ShippingMethod
    {
        [Key]
        public int Id { get; set; }
        public ShippingType Type { get; set; }
        public double ShippingCost { get; set; }
        public int EstimatedDeliveryTime { get; set; }
        public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();


    }
}

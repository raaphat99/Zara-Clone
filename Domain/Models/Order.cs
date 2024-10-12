using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public string? UserId { get; set; }
        public virtual User User { get; set; }

        [ForeignKey("ShippingMethod")]
        public int? ShippingMethodId { get; set; }
        public virtual ShippingMethod ShippingMethod { get; set; }

        [ForeignKey("UserAddress")]
        public int? UserAddressId { get; set; }
        public virtual UserAddress UserAddress { get; set; }
        public virtual Payment Payment { get; set; }
        public string? TrackingNumber { get; set; }
        public OrderStatus Status { get; set; }
        public double TotalPrice { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();

    }
}

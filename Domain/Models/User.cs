using Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? LastUpdated { get; set; }
        public virtual ICollection<UserAddress> Adresses { get; set; } = new List<UserAddress>();
        public virtual ICollection<UserMeasurement> UserMeasurements { get; set; } = new List<UserMeasurement>();
        public virtual Cart Cart { get; set; }
        public virtual Wishlist Wishlist { get; set; }
        public virtual Notification Notification { get; set; }
    }
}

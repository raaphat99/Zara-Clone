using Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class User:IdentityUser
    {
        public DateTime? Created { get; set; }
        public DateTime? LastUpdated { get; set; }   
        public virtual ICollection<UserAddress>? Adresses { get; set; }
    }
}

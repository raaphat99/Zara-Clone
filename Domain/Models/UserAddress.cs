using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class UserAddress
    {
        [Key]
        public int Id { get; set; }
        public string? Country { get; set; }
        public string? State { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }
        public string? AddressMoreInfo { get; set; }
        public string PhoneNumber { get; set; }
        public bool? Active { get; set; } = false;

        [ForeignKey("User")]
        public string? UserId { get; set; }
        public virtual User User { get; set; }
    }
}

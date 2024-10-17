using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class TrackingNumber
    {
        [Key]
        public int Id { get; set; }
        public int UniversalTrackingNumber { get; set; }

    }
}

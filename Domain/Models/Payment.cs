using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Order")]
        public int OrderId {  get; set; }
        public virtual Order Order { get; set; }
        public string PaymentMethod { get; set; }

        public string PaymentStatus {  get; set; }  
        public decimal Amount {  get; set; }
        public decimal AmountRefunded { get; set; }
        public DateTime Created {  get; set; }
        public DateTime Updated { get; set; }
    }
}

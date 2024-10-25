﻿using Domain.Enums;
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
        public int? OrderId { get; set; }
        public virtual Order Order { get; set; }
        public string PaymentMethod { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
        public double Amount { get; set; }
        public double AmountRefunded { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
    }
}

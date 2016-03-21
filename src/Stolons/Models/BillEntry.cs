﻿
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Stolons.Models
{
    public class BillEntry
    {
        [Key]
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public User User { get; set; }
        [Display(Name = "Fiche produit")]
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        [Display(Name = "Quantité")]
        public int Quantity { get; set; }

        public BillEntry Clone()
        {
            BillEntry clonedBillEntry = new BillEntry();
            clonedBillEntry.ProductId = this.ProductId;
            clonedBillEntry.User = this.User;
            clonedBillEntry.Product = this.Product;
            clonedBillEntry.Quantity = this.Quantity;
            return clonedBillEntry;
        }
    }
}

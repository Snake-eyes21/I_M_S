using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Ims.Models
{
    public class CreditManagement
    {
        [Key]
        public int CreditId { get; set; }

        [ForeignKey("Users")]
        public string UserId { get; set; }

        [ForeignKey("Suppliers")]
        public int SupplierId { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public Decimal OutstandingBalance { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public Decimal CreditLimit { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.Now;
        [ValidateNever]
        public ApplicationUser Users { get; set; } // Navigation property
        [ValidateNever]
        public Supplier Suppliers { get; set; } // Navigation property
    }
}

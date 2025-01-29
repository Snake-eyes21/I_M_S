using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Identity;

namespace Ims.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        [ForeignKey("Users")]
        public string UserId { get; set; }

        [ValidateNever]
        public ApplicationUser Users { get; set; }

        [Required]
        public string OrderType { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public Decimal TotalAmount { get; set; }

        [ValidateNever]
        public string Status { get; set; }

        [ValidateNever]
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
    
}

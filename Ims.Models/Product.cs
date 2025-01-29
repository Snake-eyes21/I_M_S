using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Ims.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        [StringLength(100)]
        [DisplayName("Product Name")]
        public string? ProductName { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        [DisplayName("Unit Price")]
        public Decimal UnitPrice { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        [DisplayName("Stock Quantity")]
        public int QuantityStock { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        [DisplayName("Reorder Level")]
        public int ReorderLevel { get; set; }

        [ForeignKey("CategoryId")]
        public int CategoryId { get; set; }
        [ValidateNever]
        public Category Category { get; set; }

        [ValidateNever]
        public string? ImageUrl {  get; set; }
    }
}

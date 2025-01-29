using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ims.Models.ViewModels;
using static NuGet.Packaging.PackagingConstants;

namespace Ims.Models
{
    public class OrderDetail
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int SerialId { get; set; }
        [ForeignKey("Orders")]
        public int OrderId { get; set; }

        [ForeignKey("Products")]
        public int ProductId { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public Decimal Price { get; set; }

        public Order Orders { get; set; } // Navigation property
        public Product Products { get; set; } // Navigation property
    }
}

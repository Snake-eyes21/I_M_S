using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ims.Models.ViewModels
{
    public class CreateOrderViewModel
    {
        

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal Price { get; set; }
        [Required]
        public string UserId {  get; set; }
        [Required]
        public OrdersType OrderType { get; set; }
        
    }
    public enum OrdersType
    {
        Online,
        Instore
    }
}

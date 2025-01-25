using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Ims.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(100)]
        [DisplayName("Category Name")]
        public string? CategoryName { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PracticeWebProjects.Data.Models
{
    public class Sale
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public int DishId { get; set; }

        [Required]
        public decimal Income { get; set; }

        public DateTime TransactionDate { get; set; }


    }
}

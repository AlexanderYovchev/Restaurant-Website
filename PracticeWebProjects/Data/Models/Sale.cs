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
        [ForeignKey(nameof(DishId))]
        public virtual Dish Dish { get; set; } = null!;

        public DateTime TransactionDate { get; set; }


    }
}

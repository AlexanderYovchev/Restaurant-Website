using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PracticeWebProjects.Data.Models
{
    public class DishChef
    {
        [Required]
        [Key]
        public int ChefId { get; set; }

        [Required]
        [ForeignKey(nameof(ChefId))]
        public Chef Chef { get; set; } = null!;

        [Required]
        [Key]
        public int DishId { get; set; }

        [Required]
        [ForeignKey(nameof(DishId))]
        public Dish Dish { get; set; } = null!;
    }
}

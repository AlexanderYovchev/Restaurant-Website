using System.ComponentModel.DataAnnotations;
using static PracticeWebProjects.DataValidatingClass;

namespace PracticeWebProjects.Data.Models
{
    public class DishType
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(dishTypeMaxLength)]
        public string Name { get; set; } = string.Empty;

        public ICollection<Dish> Dishes { get; set; } = new List<Dish>();
    }
}

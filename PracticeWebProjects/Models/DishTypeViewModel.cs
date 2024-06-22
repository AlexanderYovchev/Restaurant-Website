using System.ComponentModel.DataAnnotations;
using static PracticeWebProjects.DataValidatingClass;

namespace PracticeWebProjects.Models
{
    public class DishTypeViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(dishTypeMaxLength, MinimumLength = dishTypeMinLength, ErrorMessage = "Dish {0} must be between {2} and {1} characters long.")]
        public string Name { get; set; } = string.Empty;
    }
}

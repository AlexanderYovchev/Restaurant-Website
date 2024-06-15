using System.ComponentModel.DataAnnotations;
using static PracticeWebProjects.DataValidatingClass;
namespace PracticeWebProjects.Models
{
    public class DishChefsViewModel
    {
        [StringLength(chefNameMaxLength, MinimumLength = chefNameMinLength,ErrorMessage = "Chef {0} must be between {2} and {1} characters long.")]
        public string ChefName { get; set; } = string.Empty;

        public string DishName { get; set; } = string.Empty;
    }
}

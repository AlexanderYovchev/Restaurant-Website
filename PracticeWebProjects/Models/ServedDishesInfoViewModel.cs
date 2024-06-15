using PracticeWebProjects.Data.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using static PracticeWebProjects.DataValidatingClass;

namespace PracticeWebProjects.Models
{
    public class ServedDishesInfoViewModel
    {
        public int Id { get; set; }

        [StringLength(dishNameMaxLength, MinimumLength = dishNameMinLength, ErrorMessage = "Dish {0} must be between {2} and {1} characters long.")]
        public string Name { get; set; } = string.Empty;

        public decimal Cost { get; set; }

        public string DishType { get; set; } = string.Empty;


        public bool IsServed { get; set; }


        public ICollection<DishChefsViewModel> DishChefs { get; set; } = new List<DishChefsViewModel>();
    }
}

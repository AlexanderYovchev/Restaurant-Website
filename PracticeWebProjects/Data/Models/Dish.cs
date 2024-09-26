using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static PracticeWebProjects.DataValidatingClass;

namespace PracticeWebProjects.Data.Models
{
    public class Dish
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(dishNameMaxLength)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public decimal Cost { get; set; }

        [Required]
        public int DishTypeId { get; set; }

        [Required]
        [ForeignKey(nameof(DishTypeId))]
        public DishType DishType { get; set; } = null!;

        [Required]
        public int ServingTableId { get; set; }

        [Required]
        [ForeignKey(nameof(ServingTableId))]
        public ServingTable ServingTable { get; set; } = null!;

        [Required]
        public bool IsServed { get; set; } = false;

        public ICollection<DishChef> DishChefs { get; set; } = new List<DishChef>();

    }
}

using System.ComponentModel.DataAnnotations;

namespace PracticeWebProjects.Data.Models
{
    public class ServingTable
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public decimal TotalIncome { get; set; }

        [Required]
        public bool isReserved { get; set; } = false;

        [Required]
        public bool isTaken { get; set; } = false;

        public ICollection<Dish> ServedDishes { get; set; } = new List<Dish>();
    }
}

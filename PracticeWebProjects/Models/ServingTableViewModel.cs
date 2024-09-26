using PracticeWebProjects.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace PracticeWebProjects.Models
{
    public class ServingTableViewModel
    {
        
        public int Id { get; set; }

        
        public decimal TotalIncome { get; set; }

        
        public bool isReserved { get; set; } = false;

        
        public bool isTaken { get; set; } = false;

        public ICollection<Dish> ServedDishes { get; set; } = new List<Dish>();
    }
}

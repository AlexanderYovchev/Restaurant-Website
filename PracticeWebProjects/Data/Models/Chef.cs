using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using static PracticeWebProjects.DataValidatingClass;

namespace PracticeWebProjects.Data.Models
{
    public class Chef
    {
        [Key]
        [Required]
        [Comment("Chef id")]
        public int Id { get; set; }

        [Required]
        [StringLength(chefNameMaxLength)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(chefMinAge, chefMaxAge)]
        public int Age { get; set; }

        [Required]
        [Range(chefMinSalary, chefMaxSalary)]
        public int Salary { get; set; }



        public ICollection<DishChef> DishChefs { get; set; } = new List<DishChef>();
    }
}

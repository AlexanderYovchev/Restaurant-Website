using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using static PracticeWebProjects.DataValidatingClass;

namespace PracticeWebProjects.Models
{
    public class ChefsDisplayViewModel
    {
        [Required]
        [Comment("Chef id")]
        public int Id { get; set; }


        [StringLength(chefNameMaxLength, MinimumLength = chefNameMinLength, 
            ErrorMessage = "Chef {0} must be between {2} and {1} characters long.")]
        public string Name { get; set; } = string.Empty;

        [Range(chefMinAge, chefMaxAge)]
        public int Age { get; set; }


        [Range(chefMinSalary, chefMaxSalary)]
        public int Salary { get; set; }
    }
}

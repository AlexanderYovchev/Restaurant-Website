﻿using PracticeWebProjects.Data.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using static PracticeWebProjects.DataValidatingClass;

namespace PracticeWebProjects.Models
{
    public class DishFromViewModel
    {
        
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(dishNameMaxLength,MinimumLength = dishNameMinLength, ErrorMessage = "Dish {0} must be between {2} and {1} characters long.")]
        public string Name { get; set; } = string.Empty;

        [Required]
        public decimal Cost { get; set; }

        [Required]
        public int DishTypeId { get; set; }

        [Required]
        [ForeignKey(nameof(DishTypeId))]
        public DishType DishType { get; set; } = null!;

        [Required]
        public bool IsServed { get; set; } = false;

        public ICollection<DishChef> DishChefs { get; set; } = new List<DishChef>();
    }
}

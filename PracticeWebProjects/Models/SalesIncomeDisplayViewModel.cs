using PracticeWebProjects.Data.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PracticeWebProjects.Models
{
    public class SalesIncomeDisplayViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; }

        [Required]
        public decimal Income { get; set; }

    }
}

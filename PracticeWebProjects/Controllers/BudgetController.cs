using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PracticeWebProjects.Data;
using PracticeWebProjects.Models;
using PracticeWebProjects.Services;
using System.Globalization;

namespace PracticeWebProjects.Controllers
{
    public class BudgetController : Controller
    {
        private readonly BudgetService budgetService;
        private readonly ILogger<BudgetController> logger;

        public BudgetController(BudgetService _budgetService, ILogger<BudgetController> _logger)
        {
            budgetService = _budgetService;
            logger = _logger;
        }

        [HttpGet]
        public IActionResult DailyIncome()
        {

            return View();
        }

        [HttpPost]
        public IActionResult DailyIncome(string date)
        {
            DateTime transactionDate;

            if (!DateTime.TryParseExact(
                date, 
                DataValidatingClass.saleDateFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out transactionDate))
            {
                ModelState.AddModelError(date, 
                    $"Date is not in the correct format. Format must be {DataValidatingClass.saleDateFormat}");
                return View();
            }

            var income = budgetService.GetDailyIncome(transactionDate);

            var model = new SalesIncomeDisplayViewModel()
            {
                TransactionDate = transactionDate,
                Income = income,
            };

            return View("DailyIncomeResult",model);
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}

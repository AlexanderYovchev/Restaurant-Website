using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PracticeWebProjects.CustomModelBinders;
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
        public IActionResult DailyIncome([ModelBinder(BinderType = typeof(CustomDateTimeModelBinder))] DateTime date)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var income = budgetService.GetDailyIncome(date);

            var model = new SalesIncomeDisplayViewModel()
            {
                TransactionDate = date,
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

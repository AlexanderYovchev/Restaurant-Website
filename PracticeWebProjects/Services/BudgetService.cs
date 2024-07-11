using PracticeWebProjects.Data;

namespace PracticeWebProjects.Services
{
    public class BudgetService
    {
        private readonly ApplicationDbContext context;

        public BudgetService(ApplicationDbContext _context)
        {
            context = _context;
        }

        public decimal GetDailyIncome(DateTime date)
        {
            var startOfDay = date.Date;
            var endOfDay = date.AddDays(1).AddTicks(-1);

            return context.Sales
                .Where(s => s.TransactionDate >= startOfDay && s.TransactionDate <= endOfDay)
                .Sum(s => s.Dish.Cost);
        }

        public decimal GetMonthlyIncome(int year, int month)
        {
            var startOfMonth = new DateTime(year, month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddTicks(-1);


            //Calculating the income for the month
            var totalIncome = context.Sales
                .Where(s => s.TransactionDate >= startOfMonth && s.TransactionDate <= endOfMonth)
                .Sum(s => s.Dish.Cost);

            //Calculating the expenses for the month
            var totalExpenses = context.Chefs.Sum(c => c.Salary);

            return totalIncome - totalExpenses;
        }
    }
}

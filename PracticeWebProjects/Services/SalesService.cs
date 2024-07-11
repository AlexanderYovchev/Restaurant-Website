using Microsoft.AspNetCore.Mvc;
using PracticeWebProjects.Data;
using PracticeWebProjects.Data.Models;

namespace PracticeWebProjects.Services
{
    public class SalesService
    {
        private readonly ApplicationDbContext context;

        public SalesService(ApplicationDbContext _context)
        {
            context = _context;
        }

        public async Task RegisterSale(int dishId)
        {
            var sale = new Sale
            {
                DishId = dishId,
                TransactionDate = DateTime.Now,
            };

            context.Sales.Add(sale);
            await context.SaveChangesAsync();

        }
    }
}

using Microsoft.EntityFrameworkCore;
using PracticeWebProjects.Data;
using PracticeWebProjects.Models;

namespace PracticeWebProjects.Services
{
    public class ChefService
    {
        private readonly ApplicationDbContext context;

        public ChefService(ApplicationDbContext _context)
        {
            context = _context;
        }

        public async Task<List<ChefsDisplayViewModel>> GetChefsAsync()
        {
            return await context.Chefs
                .Select(c => new ChefsDisplayViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Age = c.Age,
                })
                .AsNoTracking()
                .ToListAsync();
        }
    }
}

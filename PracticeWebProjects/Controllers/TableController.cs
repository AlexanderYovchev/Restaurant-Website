using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PracticeWebProjects.Data;
using PracticeWebProjects.Data.Models;
using PracticeWebProjects.Models;

namespace PracticeWebProjects.Controllers
{
    public class TableController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly ILogger<TableController> logger;
        public TableController(ILogger<TableController> _logger, ApplicationDbContext _context)
        {
            logger = _logger;
            context = _context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = context.ServingTables
                .Select(t => new ServingTableViewModel
                {
                    Id = t.Id,
                    isReserved = t.isReserved,
                    isTaken = t.isTaken,
                })
                .ToList();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTableStates([FromBody] Dictionary<int, string> tableStates)
        {
            foreach (var tableState in tableStates)
            {
                var table = await context.ServingTables.FindAsync(tableState.Key);
                if (table != null)
                {
                    switch (tableState.Value)
                    {
                        case "empty":
                            table.isReserved = false;
                            table.isTaken = false;
                            break;
                        case "reserved":
                            table.isReserved = true;
                            table.isTaken = false;
                            break;
                        case "taken":
                            table.isReserved = false;
                            table.isTaken = true;
                            break;
                    }
                }
            }

            await context.SaveChangesAsync();
            return Ok();
        }
    }
}

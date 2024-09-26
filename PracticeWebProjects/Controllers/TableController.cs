using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PracticeWebProjects.Data;
using PracticeWebProjects.Models;

namespace PracticeWebProjects.Controllers
{
    public class TableController : Controller
    {
        private readonly DbContext _context;
        private readonly ILogger<TableController> _logger;
        public TableController(ILogger<TableController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = new ServingTableViewModel
            {

            };
            return View();
        }
    }
}

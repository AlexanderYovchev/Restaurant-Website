using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PracticeWebProjects.Data;
using PracticeWebProjects.Models;
using PracticeWebProjects.Services;

namespace PracticeWebProjects.Controllers
{
    public class ChefController : Controller
    {
        private readonly ILogger<ChefController> logger;
        private readonly ApplicationDbContext context;
        private readonly ChefService chefService;

        public ChefController(ILogger<ChefController> _logger, ApplicationDbContext _context, ChefService _chefService)
        {
            logger = _logger;
            context = _context;
            chefService = _chefService;
        }

        [HttpGet]
        public async Task<IActionResult> ChefsList()
        {
            var model = await chefService.GetChefsAsync();

            return PartialView("_ChefsListDisplay", model);
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}

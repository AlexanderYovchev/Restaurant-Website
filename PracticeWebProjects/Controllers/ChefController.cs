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

        [HttpGet]
        public IActionResult CreateChef()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateChef(ChefsDisplayViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (context.Chefs.Any(c => c.Id == model.Id && c.Name == model.Name))
            {
                ModelState.AddModelError("InvalidChefError", "Chef already exist!");
            }

            await chefService.CreateChefAsync(model);

            return RedirectToAction(nameof(Index), nameof(HomeController));
        }


        public IActionResult Index()
        {
            return View();
        }
    }
}

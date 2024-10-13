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
        public async Task<IActionResult> ChefsList(int pg = 1)
        {
            var model = await chefService.GetChefsAsync();
            const int pageSize = 4;
            if (pg < 1)
            {
                pg = 1;
            }
            
            int recsCount = model.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (int)(pg - 1) * pageSize;
            var data = model.Skip(recSkip).Take(pager.PageSize).ToList();

            ViewData["Pager"] = pager;


            return RedirectToAction("Index", "Home", new { chefs = data, pg });
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

            if (context.Chefs.Any(c => c.Name == model.Name))
            {
                ModelState.AddModelError("InvalidChefError", "Chef already exist!");
                return View();
            }

            await chefService.CreateChefAsync(model);

            return RedirectToAction(nameof(Index), "Home");
        }


        public IActionResult Index()
        {
            return View();
        }
    }
}

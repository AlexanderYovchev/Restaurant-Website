using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PracticeWebProjects.Data;
using PracticeWebProjects.Data.Models;
using PracticeWebProjects.Models;
using System.Diagnostics;

namespace PracticeWebProjects.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly ApplicationDbContext context;

        public HomeController(ILogger<HomeController> _logger, ApplicationDbContext _context)
        {
            logger = _logger;
            context = _context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        [HttpGet]
        public IActionResult CreateOrder()
        {
            ViewData["DishTypes"] = new SelectList(context.DishTypes, "Id", "Name");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrder(DishFromViewModel model)
        {
            if (ModelState.IsValid)
            {
                var dish = new Dish
                {
                    Name = model.Name,
                    DishTypeId = model.DishTypeId,
                    Cost = model.Cost,
                    IsServed = model.IsServed,
                    
                };

                var chefNames = model.DishChefs.Select(dc => dc.ChefName).Split(new[] { ", " }, System.StringSplitOptions.RemoveEmptyEntries);
                foreach (var name in chefNames)
                {
                    var chef = context.Chefs.SingleOrDefault(c => c.Name == name) ?? new Chef { Name = name };
                    dish.DishChefs.Add(new DishChef { Chef = chef });
                }

                

                context.Dishes.Add(dish);
                await context.SaveChangesAsync();

                return RedirectToAction(nameof(AwaitingOrders));


            }

            ViewData["DishTypes"] = new SelectList(context.DishTypes, "Id", "Name", model.DishTypeId);
            return View(model);

        }

        [HttpGet]
        public async Task<IActionResult> AwaitingOrders()
        {
            var model = await context.Dishes
                .Select(d => new ServedDishesInfoViewModel
                {
                    Id = d.Id,
                    Name = d.Name,
                    Cost = d.Cost,
                    DishType = d.DishType.Name,
                    IsServed = d.IsServed,
                    DishChefs = d.DishChefs.Select(dc => new DishChefsViewModel()
                    {
                        ChefName = dc.Chef.Name,
                        DishName = dc.Dish.Name,

                    }).ToList()
                })
                .AsNoTracking()
                .ToListAsync();

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> ServedOrders()
        {
            var model = await context.Dishes
                .Select(d => new ServedDishesInfoViewModel
                {
                    Id = d.Id,
                    Name = d.Name,
                    Cost = d.Cost,
                    DishType = d.DishType.Name,
                    IsServed = d.IsServed,
                    DishChefs = d.DishChefs.Select(dc => new DishChefsViewModel()
                    {
                        ChefName = dc.Chef.Name,
                        DishName = dc.Dish.Name,

                    }).ToList()
                })
                .AsNoTracking()
                .ToListAsync();

            return View(model);
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

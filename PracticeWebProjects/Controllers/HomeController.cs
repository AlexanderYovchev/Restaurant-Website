using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PracticeWebProjects.Data;
using PracticeWebProjects.Data.Models;
using PracticeWebProjects.Models;
using System.Diagnostics;
using System.Xml.Linq;

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
            var model = new DishFromViewModel
            {
                
                DishChefs = context.Chefs.Select(c => new DishChefsViewModel
                {
                    ChefName = c.Name,
                    ChefId = c.Id,

                }).ToList()
            };

            ViewData["DishTypes"] = new SelectList(context.DishTypes, "Id", "Name");
            ViewData["Chefs"] = new SelectList(model.DishChefs, "ChefId", "ChefName");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(DishFromViewModel model)
        {
            var dishType = await context.DishTypes.FindAsync(model.DishTypeId);
            if (dishType != null)
            {
                model.DishType = dishType.Name;
            }
            else
            {
                ModelState.AddModelError("DishTypeId", "Invalid Dish Type");
            }


            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }

                ViewData["DishTypes"] = new SelectList(context.DishTypes, "Id", "Name", model.DishTypeId);
                ViewData["Chefs"] = new SelectList(context.Chefs, "Id", "Name", model.SelectedChefIds);
                // This will log the errors to the console or you can use other logging mechanisms
            }

            if (ModelState.IsValid)
            {
                var dish = new Dish
                {
                    Name = model.Name,
                    DishTypeId = model.DishTypeId,
                    Cost = model.Cost,
                    IsServed = model.IsServed,
                    DishChefs = new List<DishChef>()
                };

                foreach (var chefId in model.SelectedChefIds)
                {
                    var chef = await context.Chefs.FindAsync(chefId);

                    if (chef == null)
                    {
                        return BadRequest();
                    }

                    dish.DishChefs.Add(new DishChef { Chef = chef });
                }

                context.Dishes.Add(dish);
                await context.SaveChangesAsync();

                return RedirectToAction(nameof(AwaitingOrders));


            }

            ViewData["DishTypes"] = new SelectList(GetDishTypes(), "Id", "Name", model.DishTypeId);
            ViewData["Chefs"] = new SelectList(context.Chefs, "Id", "Name", model.SelectedChefIds);
            return View(model);

        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id)
        {
            var order = await context.Dishes.
                Where(d => d.Id == id)
                .Include(dc => dc.DishChefs)
                .FirstOrDefaultAsync();

            if (order == null || order.IsServed == true)
            {
                return BadRequest();
            }

            order.IsServed = true;

            await context.SaveChangesAsync();

            return RedirectToAction("ServedOrders", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteServedOrders()
        {
            var model = await context.Dishes
                .Where(d => d.IsServed == true)
                .ToListAsync();

            if (!model.Any())
            {
                return BadRequest();
            }

            context.Dishes.RemoveRange(model);

            await context.SaveChangesAsync();

            return RedirectToAction(nameof(ServedOrders));
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

        private IList<DishTypeViewModel> GetDishTypes()
        {
            var dishTypes = context.DishTypes.Select(d => new DishTypeViewModel()
            {
                Id = d.Id,
                Name = d.Name,
            }).ToList();

            return dishTypes;
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

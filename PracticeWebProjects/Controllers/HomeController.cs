using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.EntityFrameworkCore;
using PracticeWebProjects.Data;
using PracticeWebProjects.Data.Models;
using PracticeWebProjects.Models;
using PracticeWebProjects.Services;
using System.Diagnostics;
using System.Xml.Linq;

namespace PracticeWebProjects.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly ApplicationDbContext context;
        private readonly ChefService chefService;

        public HomeController(ILogger<HomeController> _logger, ApplicationDbContext _context, ChefService _chefService)
        {
            logger = _logger;
            context = _context;
            chefService = _chefService;
        }

        public async Task<IActionResult> Index(int pg = 1)
        {
            var model = await chefService.GetChefsAsync();

            const int pageSize = 4;
            if (pg < 1)
            {
                pg = 1;
            }

            int recsCount = model.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var paginatedData = model.Skip(recSkip).Take(pageSize).ToList();

            ViewData["Pager"] = pager; 

            return View(paginatedData);
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
            ViewData["ServingTables"] = new SelectList(GetTakenServingTables(), "Id", "Id");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(DishFromViewModel model)
        {
            var dishType = await context.DishTypes.FindAsync(model.DishTypeId);
            var tableId = await context.ServingTables.FindAsync(model.ServingTableId);

            if (dishType != null)
            {
                model.DishType = dishType.Name;
            }
            else
            {
                ModelState.AddModelError("DishTypeId", "Invalid Dish Type");
            }

            if (tableId != null && tableId.isTaken == true)
            {
                model.ServingTableId = tableId.Id;
            }
            else
            {
                ModelState.AddModelError("ServingTableId", "Dishes cannot be served to empty or reserved table");
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
                ViewData["ServingTables"] = new SelectList(context.ServingTables, "Id", "Number", model.ServingTableId);
            }

            if (ModelState.IsValid)
            {
                var dish = new Dish
                {
                    Name = model.Name,
                    DishTypeId = model.DishTypeId,
                    ServingTableId = model.ServingTableId,
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
            ViewData["ServingTables"] = new SelectList(GetTakenServingTables(), "Id", "Number", model.ServingTableId);
            return View(model);

        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id)
        {
            var order = await context.Dishes.
                Where(d => d.Id == id)
                .Include(d => d.ServingTable)
                .Include(dc => dc.DishChefs)
                .FirstOrDefaultAsync();

            if (order == null || order.IsServed == true)
            {
                return BadRequest();
            }

            order.IsServed = true;
            order.ServingTable.TotalIncome += order.Cost;

            Sale sale = new Sale()
            {
                TransactionDate = DateTime.Now,
                Income = order.Cost
            };

            context.Sales.Add(sale);

            await context.SaveChangesAsync();

            return RedirectToAction("ServedOrders", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveAwaitingOrder(int id)
        {
            var order = await context.Dishes
                .Where(d => d.Id == id)
                .Include(dc => dc.DishChefs)
                .FirstOrDefaultAsync();


            if (order == null || order.IsServed == true)
            {
                return BadRequest();
            }

            context.Dishes.Remove(order);
            await context.SaveChangesAsync();

            return RedirectToAction("AwaitingOrders");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteServedOrders()
        {
            var model = await context.Dishes
                .Where(d => d.IsServed == true)
                .ToListAsync();

            if (!model.Any())
            {
                return RedirectToAction("ServedOrders");
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

        private IList<ServingTableViewModel> GetTakenServingTables()
        {
            var servingTables = context.ServingTables
                .Where(t => t.isTaken == true)
                .Select(d => new ServingTableViewModel()
            {
                Id = d.Id,
                isTaken = d.isTaken,
                isReserved = d.isReserved,

            }).ToList();

            return servingTables;
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}


using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using MyApp.Tests.PseudoData;
using NUnit.Framework;
using PracticeWebProjects.Controllers;
using PracticeWebProjects.Data;
using PracticeWebProjects.Data.Models;
using PracticeWebProjects.Models;
using PracticeWebProjects.Services;
using System.Xml.Linq;

namespace MyApp.Tests.ControllerTests
{
    [TestFixture]
    public class HomeControllerTests : ControllerBaseTest<HomeController, ChefService>
    {

        protected override ChefService CreateService()
        {
            return new ChefService(context);
        }

        protected override HomeController CreateController()
        {
            return new HomeController(loggerMock.Object, context, service);
        }


        [Test]
        public void InMemoryData_FetchedProperly()
        {
            bool[] containsData = { context.Chefs.Any(c => c.Name == "ChefA"),
                                    context.Dishes.Any(d => d.Name == "Shopska Salad"),
                                    context.DishTypes.Count() == 5,
                                    context.DishesChefs.All(dc => dc != null)};
            Assert.Multiple(() =>
            {
                foreach (var predicate in containsData)
                {
                    Assert.That(predicate, Is.EqualTo(true));
                }
            });
            
        }

        //Check IActionResult Index method
        [Test]
        public void Index_GET_ReturnsViewModel()
        {
            context.ChangeTracker.Clear();
            context.Chefs.Add(new Chef { Id = 4, Name = "Gordon Ramsay" });
            context.SaveChanges();

            int pgTest = 1;
            var result = controller.Index(pgTest);

            var viewResult = result.Result as ViewResult;

            Assert.That(viewResult, Is.Not.Null);
            var model = viewResult.Model as ChefsDisplayViewModel;
            Assert.That(viewResult, Is.InstanceOf<ViewResult>());
        }

        //Check IActionResult CreateOrder method
        [Test]
        public void CreateOrder_GET_ReturnsViewModelWithCorrectData()
        {
            var model = new DishFromViewModel
            {

                DishChefs = context.Chefs.Select(c => new DishChefsViewModel
                {
                    ChefName = c.Name,
                    ChefId = c.Id,

                }).ToList()
            };

            Assert.That(model, Is.Not.Null);
            Assert.That(model, Is.InstanceOf<DishFromViewModel>());
            Assert.That(model.DishChefs, Is.InstanceOf<ICollection<DishChefsViewModel>>());
            Assert.That(model.DishChefs, Is.Not.Null);
            Assert.That(model.DishChefs.Count, Is.GreaterThan(0));

            var result = controller.CreateOrder();
            var viewResult = result as ViewResult;

            Assert.That(viewResult, Is.Not.Null);
            Assert.That(viewResult, Is.InstanceOf<ViewResult>());

            var dishTypes = viewResult.ViewData["DishTypes"] as SelectList;
            var chefs = viewResult.ViewData["Chefs"] as SelectList;
            var servingTables = viewResult.ViewData["ServingTables"] as SelectList;

            Assert.That(dishTypes, Is.Not.Null);
            Assert.That(chefs, Is.Not.Null);
            Assert.That(servingTables, Is.Not.Null);
        }

        [Test]
        public async Task CreateOrder_POST_InvalidDishType_ReturnsViewWithErrors()
        {
            var model = new DishFromViewModel
            {
                DishTypeId = 999,
                ServingTableId = 1,
                SelectedChefIds = new List<int> { 1, 2 }
            };

            var result = await controller.CreateOrder(model);

            var viewResult = result as ViewResult;

            Assert.That(viewResult, Is.Not.Null);

            Assert.That(controller.ModelState.IsValid, Is.False);
            Assert.That(controller.ModelState["DishTypeId"].Errors, Has.Count.GreaterThan(0));

            var dishTypes = viewResult.ViewData["DishTypes"] as SelectList;
            Assert.That(dishTypes, Is.Not.Null);

        }

        [Test]
        public async Task CreateOrder_POST_InvalidServingTable()
        {
            var model = new DishFromViewModel
            {
                DishTypeId = 3,
                ServingTableId = 10,
                SelectedChefIds = new List<int> { 1 }
            };

            var result = await controller.CreateOrder(model);
            var viewResult = result as ViewResult;

            var servingTable = await context.ServingTables.FirstOrDefaultAsync(st => st.Id == model.ServingTableId);

            Assert.That(viewResult, Is.Not.Null);
            Assert.That(controller.ModelState.IsValid, Is.False);
            Assert.That(servingTable, Is.Null);
            Assert.That(controller.ModelState["ServingTableId"].Errors, Has.Count.GreaterThan(0));
        }

        [Test]
        public async Task CreateOrder_POST_ValidModel_RedirectToAwaitingOrders()
        {
            
            var model = new DishFromViewModel
            {
                Id = 1,
                DishTypeId = 1,
                ServingTableId = 1,
                SelectedChefIds = new List<int> { 1 },
                Name = "Test Dish",
                Cost = 20,
            };

            var servingTable = context.ServingTables.First(st => st.Id == model.ServingTableId);
            servingTable.isTaken = true;
            context.ServingTables.Update(servingTable);
            context.SaveChanges();

            var result = await controller.CreateOrder(model);
            var redirectResult = result as RedirectToActionResult;

            Assert.That(controller.ModelState.IsValid, Is.True);
            Assert.That(redirectResult, Is.Not.Null);
            Assert.That(redirectResult.ActionName, Is.EqualTo(nameof(HomeController.AwaitingOrders)));
        }

        [Test]
        public async Task Edit_POST_ReturnsViewModelWithData()
        {
            int id = 1;
            var order = await context.Dishes.
                Where(d => d.Id == id)
                .Include(d => d.ServingTable)
                .Include(dc => dc.DishChefs)
                .FirstOrDefaultAsync();

            Assert.That(order, Is.Not.Null);
            Assert.That(order.IsServed, Is.False);

            var result = controller.Edit(id);
            var viewResult = await result as RedirectToActionResult;

            Assert.That(viewResult, Is.Not.Null);
            Assert.That(viewResult.ActionName, Is.EqualTo(nameof(HomeController.ServedOrders)));

            order.IsServed = true;
            order.ServingTable.TotalIncome += order.Cost;

            var sale = new Sale
            {
                TransactionDate = DateTime.Now,
                Income = order.Cost
            };

            Assert.That(sale, Is.Not.Null);

            context.Sales.Add(sale);

            await context.SaveChangesAsync();

            Assert.That(context.Sales.Count, Is.GreaterThan(0));

        }

        [Test]
        public async Task Edit_POST_ReturnErrorWithInvalidData()
        {
            int id = 56;
            var order = await context.Dishes.
                Where(d => d.Id == id)
                .Include(d => d.ServingTable)
                .Include(dc => dc.DishChefs)
                .FirstOrDefaultAsync();

            var result = await controller.Edit(id);
            var badRequestResult = result as BadRequestResult;

            Assert.That(order, Is.Null);
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));

            var servedOrder = new Dish
            {
                Id = 56,
                IsServed = true,
                Cost = 100,
                ServingTable = new ServingTable { TotalIncome = 0 },
                DishChefs = new List<DishChef>()
            };
            servedOrder.IsServed = true;
            result = await controller.Edit(56);
            
            badRequestResult = result as BadRequestResult;

            Assert.That(badRequestResult.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
            
        }

        [Test]
        public async Task RemoveAwaitingOrder_POST_ReturnActionResultWithChangedContext()
        {
            int dishId = 1;
            var newDIsh = new Dish
            {
                Id = dishId,
                Name = "Shopska Salad",
                Cost = 10.20m,
                DishTypeId = 1,
                ServingTableId = 3,
                IsServed = false
            };

            var result = await controller.RemoveAwaitingOrder(dishId);

            var actionResult = result as RedirectToActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(actionResult.ActionName, Is.EqualTo(nameof(HomeController.AwaitingOrders)));

            var dish = await context.Dishes.FirstOrDefaultAsync(d => d.Id == dishId);
            Assert.That(dish, Is.Null);
        }

        [Test]
        public async Task RemoveAwaitingOrder_POST_ReturnErrorWithInvalidData()
        {
            int dishId = 56;
            var result = await controller.RemoveAwaitingOrder(dishId);
            var badRequestResult = result as BadRequestResult;

            Assert.That(badRequestResult.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));

            var newDIsh = new Dish
            {
                Id = dishId,
                Name = "Shopska Salad",
                Cost = 10.20m,
                DishTypeId = 1,
                ServingTableId = 3,
                IsServed = true
            };

            result = await controller.RemoveAwaitingOrder(dishId);
            badRequestResult = result as BadRequestResult;

            Assert.That(badRequestResult.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        }

        [Test]
        public async Task DeleteServedOrders_POST_ReturnActionResultWithChangedContext()
        {
            int dishId = 1;

            List<Dish> dishesToInsert = new List<Dish>()
            {
                new Dish
                {
                    Id = 30,
                    Name = "Shopska Salad",
                    Cost = 10.20m,
                    DishTypeId = 1,
                    ServingTableId = 3,
                    IsServed = true
                },
                new Dish
                {
                    Id = 31,
                    Name = "Shopska Salad2",
                    Cost = 10.20m,
                    DishTypeId = 1,
                    ServingTableId = 3,
                    IsServed = true
                },
                new Dish
                {
                    Id = 32,
                    Name = "Shopska Salad3",
                    Cost = 10.20m,
                    DishTypeId = 1,
                    ServingTableId = 3,
                    IsServed = true
                }
                
            };

            context.AddRange(dishesToInsert);
            context.SaveChanges();

            var insertedDishes = context.Dishes.Where(d => d.Id >= 30 && d.Id <= 32).Take(3).ToList();

            Assert.Multiple(() =>
            {
                for (int i = 0; i <= 2; i++)
                {
                    Assert.That(insertedDishes[i].IsServed, Is.True);
                }
            });

            var result = await controller.DeleteServedOrders();

            var actionResult = result as RedirectToActionResult;

            Assert.That(actionResult.ActionName, Is.EqualTo(nameof(HomeController.ServedOrders)));

            bool containsServedOrders = context.Dishes.Any(d => d.IsServed == true);

            Assert.That(containsServedOrders, Is.False);
        }

        [Test]
        public async Task AwaitingOrders_GET_ReturnsViewWithProperData()
        {
            
            var result = controller.AwaitingOrders();

            var viewResult = await result as ViewResult;

            Assert.That(viewResult, Is.Not.Null);
            var model = viewResult.Model as IEnumerable<ServedDishesInfoViewModel>;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Count(), Is.GreaterThan(0));
        } 
    }
}

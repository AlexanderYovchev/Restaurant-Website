
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
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
    }
}

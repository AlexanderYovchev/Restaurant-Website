
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using MyApp.Tests.PseudoData;
using NUnit.Framework;
using PracticeWebProjects.Controllers;
using PracticeWebProjects.Data;
using PracticeWebProjects.Data.Models;
using PracticeWebProjects.Models;
using PracticeWebProjects.Services;

namespace MyApp.Tests.ControllerTests
{
    [TestFixture]
    public class HomeControllerTests
    {
        private HomeController controller;

        private Mock<ILogger<HomeController>> loggerMock;
        private ApplicationDbContext context;
        private ChefService chefService;


        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                            .UseInMemoryDatabase(Guid.NewGuid().ToString())
                            .Options;

            loggerMock = new Mock<ILogger<HomeController>>();
            context = new ApplicationDbContext(options);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            context.ChangeTracker.Clear();

            InMemoryDbInitializer.Seed(context);
            chefService = new ChefService(context);
            controller = new HomeController(loggerMock.Object, context, chefService);
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

    }
}

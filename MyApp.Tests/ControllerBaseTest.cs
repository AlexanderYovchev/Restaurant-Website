using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using MyApp.Tests.PseudoData;
using NUnit.Framework;
using PracticeWebProjects.Controllers;
using PracticeWebProjects.Data;
using PracticeWebProjects.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Tests
{
    public abstract class ControllerBaseTest<TController, TService>
        where TController : Controller
        where TService : class
    {
        protected TController controller;
        protected TService service;
        protected Mock<ILogger<TController>> loggerMock;
        protected ApplicationDbContext context;

        [SetUp]
        public virtual void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                            .UseInMemoryDatabase(Guid.NewGuid().ToString())
                            .Options;

            loggerMock = new Mock<ILogger<TController>>();
            context = new ApplicationDbContext(options);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            context.ChangeTracker.Clear();

            InMemoryDbInitializer.Seed(context);
            service = CreateService();
            controller = CreateController();
        }

        protected abstract TService CreateService();

        protected abstract TController CreateController();
    }
}

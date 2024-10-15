using PracticeWebProjects.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Tests.PseudoData
{
    public class SeedPseudoData
    {
        public List<Chef> Chefs { get; set; }

        public List<Dish> Dishes { get; set; }

        public List<DishChef> DishesChefs { get; set; }

        public List<DishType> DishTypes { get; set; }

        public List<ServingTable> ServingTables { get; set; }
    }
}

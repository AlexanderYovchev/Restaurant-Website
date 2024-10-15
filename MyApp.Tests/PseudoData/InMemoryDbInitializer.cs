using Microsoft.EntityFrameworkCore;
using PracticeWebProjects.Data;
using System.Reflection;
using Newtonsoft.Json;
using PracticeWebProjects.Data.Models;


namespace MyApp.Tests.PseudoData
{
    public class InMemoryDbInitializer
    {
        public static void Seed(ApplicationDbContext context)
        {
            var chefCount = context.Chefs.Count();
            var dishCount = context.Dishes.Count();

            var hasData = chefCount > 0 || dishCount > 0;

            if (!hasData)
            {
                var path = Path.Combine("..","..", "..", "PseudoData", "InMemoryDbPseudoData.json");
                var jsonData = File.ReadAllText(path);
                var seedData = JsonConvert.DeserializeObject<SeedPseudoData>(jsonData);

                context.Chefs.AddRange(seedData.Chefs);
                context.Dishes.AddRange(seedData.Dishes);

                
                context.SaveChanges();
            }
        }
    }
}

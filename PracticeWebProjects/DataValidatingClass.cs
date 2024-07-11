namespace PracticeWebProjects
{
    public class DataValidatingClass
    {
        // Chefs properties validation
        public const int chefNameMaxLength = 100;

        public const int chefNameMinLength = 5;

        public const int chefMinAge = 15;

        public const int chefMaxAge = 70;

        public const int chefMinSalary = 1500;

        public const int chefMaxSalary = 8000;

        // Dishes properties validation

        public const int dishNameMaxLength = 150;

        public const int dishNameMinLength = 2;

        // Dish Type properties validation

        public const int dishTypeMaxLength = 150;

        public const int dishTypeMinLength = 2;

        //Sales DateTime format validation

        public const string saleDateFormat = "yyyy-MM-dd";
    }
}

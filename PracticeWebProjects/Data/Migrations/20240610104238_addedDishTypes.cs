using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PracticeWebProjects.Data.Migrations
{
    public partial class addedDishTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "DishTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Starter Dish" },
                    { 2, "Main Dish" },
                    { 3, "Dessert" },
                    { 4, "Side Dish" },
                    { 5, "Beverage" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "DishTypes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "DishTypes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "DishTypes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "DishTypes",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "DishTypes",
                keyColumn: "Id",
                keyValue: 5);
        }
    }
}

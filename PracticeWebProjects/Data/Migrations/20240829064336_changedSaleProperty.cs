using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PracticeWebProjects.Data.Migrations
{
    public partial class changedSaleProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Dishes_DishId",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_DishId",
                table: "Sales");

            migrationBuilder.AddColumn<decimal>(
                name: "Income",
                table: "Sales",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Income",
                table: "Sales");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_DishId",
                table: "Sales",
                column: "DishId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Dishes_DishId",
                table: "Sales",
                column: "DishId",
                principalTable: "Dishes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

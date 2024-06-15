using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PracticeWebProjects.Data.Migrations
{
    public partial class AddedServedProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsServed",
                table: "Dishes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsServed",
                table: "Dishes");
        }
    }
}

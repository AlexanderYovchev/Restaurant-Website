using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PracticeWebProjects.Data.Migrations
{
    public partial class ServingTableEntityAndRelationToDishAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ServingTableId",
                table: "Dishes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ServingTables",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TotalIncome = table.Column<decimal>(type: "decimal(9,2)", nullable: false),
                    isReserved = table.Column<bool>(type: "bit", nullable: false),
                    isTaken = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServingTables", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ServingTables",
                columns: new[] { "Id", "TotalIncome", "isReserved", "isTaken" },
                values: new object[,]
                {
                    { 1, 0m, false, false },
                    { 2, 0m, false, false },
                    { 3, 0m, false, false },
                    { 4, 0m, false, false },
                    { 5, 0m, false, false },
                    { 6, 0m, false, false },
                    { 7, 0m, false, false },
                    { 8, 0m, false, false },
                    { 9, 0m, false, false }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dishes_ServingTableId",
                table: "Dishes",
                column: "ServingTableId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dishes_ServingTables_ServingTableId",
                table: "Dishes",
                column: "ServingTableId",
                principalTable: "ServingTables",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dishes_ServingTables_ServingTableId",
                table: "Dishes");

            migrationBuilder.DropTable(
                name: "ServingTables");

            migrationBuilder.DropIndex(
                name: "IX_Dishes_ServingTableId",
                table: "Dishes");

            migrationBuilder.DropColumn(
                name: "ServingTableId",
                table: "Dishes");
        }
    }
}

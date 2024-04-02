using Microsoft.EntityFrameworkCore.Migrations;

namespace ImportExcelSql.Migrations
{
    public partial class t1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cars",
                columns: table => new
                {
                    carId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    carName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    doorNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    bodyStyle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    engineLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    numberOfCylinders = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    horsePower = table.Column<int>(type: "int", nullable: false),
                    price = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cars", x => x.carId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cars");
        }
    }
}

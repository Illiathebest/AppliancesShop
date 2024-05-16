using Microsoft.EntityFrameworkCore.Migrations;

namespace AppliancesShop_DataAccess.Migrations
{
    public partial class OrderDetailModelChanged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "OrderDetail",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "OrderDetail",
                newName: "Price");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "OrderDetail",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "OrderDetail",
                newName: "Price");
        }
    }
}

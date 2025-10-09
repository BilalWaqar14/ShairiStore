using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShairiStore.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryModelWithUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "inventories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalOrderedQuantityKgs",
                table: "inventories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_inventories_OrderId",
                table: "inventories",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_inventories_Orders_OrderId",
                table: "inventories",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_inventories_Orders_OrderId",
                table: "inventories");

            migrationBuilder.DropIndex(
                name: "IX_inventories_OrderId",
                table: "inventories");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "inventories");

            migrationBuilder.DropColumn(
                name: "TotalOrderedQuantityKgs",
                table: "inventories");
        }
    }
}

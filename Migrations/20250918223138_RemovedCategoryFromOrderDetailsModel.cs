using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShairiStore.Migrations
{
    /// <inheritdoc />
    public partial class RemovedCategoryFromOrderDetailsModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Categories_CategoryId",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_CategoryId",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "OrderDetails");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "OrderDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_CategoryId",
                table: "OrderDetails",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Categories_CategoryId",
                table: "OrderDetails",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

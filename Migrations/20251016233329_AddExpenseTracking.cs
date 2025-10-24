using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShairiStore.Migrations
{
    /// <inheritdoc />
    public partial class AddExpenseTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AmountPaid",
                table: "Expenses",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExpenseStatusId",
                table: "Expenses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InitiatedBy",
                table: "Expenses",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<double>(
                name: "RemainingAmount",
                table: "Expenses",
                type: "double",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_ExpenseStatusId",
                table: "Expenses",
                column: "ExpenseStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_InitiatedBy",
                table: "Expenses",
                column: "InitiatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_AspNetUsers_InitiatedBy",
                table: "Expenses",
                column: "InitiatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_ExpenseStatuses_ExpenseStatusId",
                table: "Expenses",
                column: "ExpenseStatusId",
                principalTable: "ExpenseStatuses",
                principalColumn: "ExpenseStatusId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_AspNetUsers_InitiatedBy",
                table: "Expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_ExpenseStatuses_ExpenseStatusId",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_ExpenseStatusId",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_InitiatedBy",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "AmountPaid",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "ExpenseStatusId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "InitiatedBy",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "RemainingAmount",
                table: "Expenses");
        }
    }
}

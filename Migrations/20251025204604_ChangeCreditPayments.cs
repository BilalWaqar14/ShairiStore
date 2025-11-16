using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShairiStore.Migrations
{
    /// <inheritdoc />
    public partial class ChangeCreditPayments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_ExpenseStatus_ExpenseStatusId",
                table: "Expenses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExpenseStatus",
                table: "ExpenseStatus");

            migrationBuilder.RenameTable(
                name: "ExpenseStatus",
                newName: "ExpenseStatuses");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExpenseStatuses",
                table: "ExpenseStatuses",
                column: "ExpenseStatusId");

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
                name: "FK_Expenses_ExpenseStatuses_ExpenseStatusId",
                table: "Expenses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExpenseStatuses",
                table: "ExpenseStatuses");

            migrationBuilder.RenameTable(
                name: "ExpenseStatuses",
                newName: "ExpenseStatus");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExpenseStatus",
                table: "ExpenseStatus",
                column: "ExpenseStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_ExpenseStatus_ExpenseStatusId",
                table: "Expenses",
                column: "ExpenseStatusId",
                principalTable: "ExpenseStatus",
                principalColumn: "ExpenseStatusId");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShairiStore.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentsInExpenseDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "CreditPayments",
                columns: table => new
                {
                    PaymentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ExpenseId = table.Column<int>(type: "int", nullable: false),
                    AmountPaid = table.Column<double>(type: "double", nullable: false),
                    RemainingAmount = table.Column<double>(type: "double", nullable: false),
                    PaymentMethod = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreditScreenshot = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PaymentDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    PaidBy = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    InvoiceStatusId = table.Column<int>(type: "int", nullable: false),
                    UpdatedBy = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditPayments", x => x.PaymentId);
                    table.ForeignKey(
                        name: "FK_CreditPayments_AspNetUsers_PaidBy",
                        column: x => x.PaidBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CreditPayments_Expenses_ExpenseId",
                        column: x => x.ExpenseId,
                        principalTable: "Expenses",
                        principalColumn: "ExpenseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CreditPayments_InvoiceStatuses_InvoiceStatusId",
                        column: x => x.InvoiceStatusId,
                        principalTable: "InvoiceStatuses",
                        principalColumn: "InvoiceStatusId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_CreditPayments_ExpenseId",
                table: "CreditPayments",
                column: "ExpenseId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditPayments_InvoiceStatusId",
                table: "CreditPayments",
                column: "InvoiceStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditPayments_PaidBy",
                table: "CreditPayments",
                column: "PaidBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_ExpenseStatus_ExpenseStatusId",
                table: "Expenses",
                column: "ExpenseStatusId",
                principalTable: "ExpenseStatus",
                principalColumn: "ExpenseStatusId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_ExpenseStatus_ExpenseStatusId",
                table: "Expenses");

            migrationBuilder.DropTable(
                name: "CreditPayments");

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
    }
}

using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShairiStore.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderPaymentWithOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderInvoices_PaymentMethods_PaymentMethodId",
                table: "OrderInvoices");

            migrationBuilder.DropIndex(
                name: "IX_OrderInvoices_PaymentMethodId",
                table: "OrderInvoices");

            migrationBuilder.DropColumn(
                name: "OrderAmount",
                table: "OrderInvoices");

            migrationBuilder.DropColumn(
                name: "PaymentMethodId",
                table: "OrderInvoices");

            migrationBuilder.AlterColumn<string>(
                name: "InvoiceBy",
                table: "OrderInvoices",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "OrderPayments",
                columns: table => new
                {
                    PaymentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    AmountPaid = table.Column<double>(type: "double", nullable: false),
                    RemainingAmount = table.Column<double>(type: "double", nullable: false),
                    PaymentMethodId = table.Column<int>(type: "int", nullable: false),
                    PaymentScreenshot = table.Column<string>(type: "longtext", nullable: false)
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
                    table.PrimaryKey("PK_OrderPayments", x => x.PaymentId);
                    table.ForeignKey(
                        name: "FK_OrderPayments_AspNetUsers_PaidBy",
                        column: x => x.PaidBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderPayments_InvoiceStatuses_InvoiceStatusId",
                        column: x => x.InvoiceStatusId,
                        principalTable: "InvoiceStatuses",
                        principalColumn: "InvoiceStatusId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderPayments_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderPayments_PaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "PaymentMethods",
                        principalColumn: "PaymentMethodId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_OrderInvoices_InvoiceBy",
                table: "OrderInvoices",
                column: "InvoiceBy");

            migrationBuilder.CreateIndex(
                name: "IX_OrderPayments_InvoiceStatusId",
                table: "OrderPayments",
                column: "InvoiceStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderPayments_OrderId",
                table: "OrderPayments",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderPayments_PaidBy",
                table: "OrderPayments",
                column: "PaidBy");

            migrationBuilder.CreateIndex(
                name: "IX_OrderPayments_PaymentMethodId",
                table: "OrderPayments",
                column: "PaymentMethodId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderInvoices_AspNetUsers_InvoiceBy",
                table: "OrderInvoices",
                column: "InvoiceBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderInvoices_AspNetUsers_InvoiceBy",
                table: "OrderInvoices");

            migrationBuilder.DropTable(
                name: "OrderPayments");

            migrationBuilder.DropIndex(
                name: "IX_OrderInvoices_InvoiceBy",
                table: "OrderInvoices");

            migrationBuilder.AlterColumn<string>(
                name: "InvoiceBy",
                table: "OrderInvoices",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<double>(
                name: "OrderAmount",
                table: "OrderInvoices",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "PaymentMethodId",
                table: "OrderInvoices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OrderInvoices_PaymentMethodId",
                table: "OrderInvoices",
                column: "PaymentMethodId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderInvoices_PaymentMethods_PaymentMethodId",
                table: "OrderInvoices",
                column: "PaymentMethodId",
                principalTable: "PaymentMethods",
                principalColumn: "PaymentMethodId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

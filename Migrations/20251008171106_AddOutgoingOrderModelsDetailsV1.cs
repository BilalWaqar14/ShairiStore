using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShairiStore.Migrations
{
    /// <inheritdoc />
    public partial class AddOutgoingOrderModelsDetailsV1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OutgoingOrders",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OrderTypeId = table.Column<int>(type: "int", nullable: false),
                    SellerId = table.Column<int>(type: "int", nullable: false),
                    OrderNotes = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OrderScreenShot = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TotalAmount = table.Column<double>(type: "double", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    OrderBy = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OrderStatusId = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    TotalOrderQuantity = table.Column<double>(type: "double", nullable: true),
                    TotalOrderKgs = table.Column<double>(type: "double", nullable: true),
                    OrderName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CustomerName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutgoingOrders", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_OutgoingOrders_AspNetUsers_OrderBy",
                        column: x => x.OrderBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutgoingOrders_OrderStatuses_OrderStatusId",
                        column: x => x.OrderStatusId,
                        principalTable: "OrderStatuses",
                        principalColumn: "StatusId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutgoingOrders_OrderTypes_OrderTypeId",
                        column: x => x.OrderTypeId,
                        principalTable: "OrderTypes",
                        principalColumn: "OrderTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutgoingOrders_Sellers_SellerId",
                        column: x => x.SellerId,
                        principalTable: "Sellers",
                        principalColumn: "SellerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutgoingOrders_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "WarehouseId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "OutgoingOrderDetails",
                columns: table => new
                {
                    OutgoingOrderDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    SubCategoryId = table.Column<int>(type: "int", nullable: false),
                    OrderQuantity = table.Column<double>(type: "double", nullable: false),
                    OrderKgs = table.Column<double>(type: "double", nullable: false),
                    TotalAmount = table.Column<double>(type: "double", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    OrderBy = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OrderStatusId = table.Column<int>(type: "int", nullable: false),
                    UpdatedBy = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutgoingOrderDetails", x => x.OutgoingOrderDetailId);
                    table.ForeignKey(
                        name: "FK_OutgoingOrderDetails_AspNetUsers_OrderBy",
                        column: x => x.OrderBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutgoingOrderDetails_OrderStatuses_OrderStatusId",
                        column: x => x.OrderStatusId,
                        principalTable: "OrderStatuses",
                        principalColumn: "StatusId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutgoingOrderDetails_OutgoingOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "OutgoingOrders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutgoingOrderDetails_SubCategories_SubCategoryId",
                        column: x => x.SubCategoryId,
                        principalTable: "SubCategories",
                        principalColumn: "SubCategoryId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "OutgoingOrderInvoices",
                columns: table => new
                {
                    InvoiceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    InvoiceAmount = table.Column<double>(type: "double", nullable: false),
                    PendingAmount = table.Column<double>(type: "double", nullable: false),
                    PaymentScreenshot = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    InvoiceDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    InvoiceBy = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    InvoiceStatusId = table.Column<int>(type: "int", nullable: false),
                    UpdatedBy = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutgoingOrderInvoices", x => x.InvoiceId);
                    table.ForeignKey(
                        name: "FK_OutgoingOrderInvoices_AspNetUsers_InvoiceBy",
                        column: x => x.InvoiceBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutgoingOrderInvoices_InvoiceStatuses_InvoiceStatusId",
                        column: x => x.InvoiceStatusId,
                        principalTable: "InvoiceStatuses",
                        principalColumn: "InvoiceStatusId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutgoingOrderInvoices_OutgoingOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "OutgoingOrders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "OutgoingOrderPayments",
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
                    table.PrimaryKey("PK_OutgoingOrderPayments", x => x.PaymentId);
                    table.ForeignKey(
                        name: "FK_OutgoingOrderPayments_AspNetUsers_PaidBy",
                        column: x => x.PaidBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutgoingOrderPayments_InvoiceStatuses_InvoiceStatusId",
                        column: x => x.InvoiceStatusId,
                        principalTable: "InvoiceStatuses",
                        principalColumn: "InvoiceStatusId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutgoingOrderPayments_OutgoingOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "OutgoingOrders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutgoingOrderPayments_PaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "PaymentMethods",
                        principalColumn: "PaymentMethodId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_OutgoingOrderDetails_OrderBy",
                table: "OutgoingOrderDetails",
                column: "OrderBy");

            migrationBuilder.CreateIndex(
                name: "IX_OutgoingOrderDetails_OrderId",
                table: "OutgoingOrderDetails",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OutgoingOrderDetails_OrderStatusId",
                table: "OutgoingOrderDetails",
                column: "OrderStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_OutgoingOrderDetails_SubCategoryId",
                table: "OutgoingOrderDetails",
                column: "SubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_OutgoingOrderInvoices_InvoiceBy",
                table: "OutgoingOrderInvoices",
                column: "InvoiceBy");

            migrationBuilder.CreateIndex(
                name: "IX_OutgoingOrderInvoices_InvoiceStatusId",
                table: "OutgoingOrderInvoices",
                column: "InvoiceStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_OutgoingOrderInvoices_OrderId",
                table: "OutgoingOrderInvoices",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OutgoingOrderPayments_InvoiceStatusId",
                table: "OutgoingOrderPayments",
                column: "InvoiceStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_OutgoingOrderPayments_OrderId",
                table: "OutgoingOrderPayments",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OutgoingOrderPayments_PaidBy",
                table: "OutgoingOrderPayments",
                column: "PaidBy");

            migrationBuilder.CreateIndex(
                name: "IX_OutgoingOrderPayments_PaymentMethodId",
                table: "OutgoingOrderPayments",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_OutgoingOrders_OrderBy",
                table: "OutgoingOrders",
                column: "OrderBy");

            migrationBuilder.CreateIndex(
                name: "IX_OutgoingOrders_OrderStatusId",
                table: "OutgoingOrders",
                column: "OrderStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_OutgoingOrders_OrderTypeId",
                table: "OutgoingOrders",
                column: "OrderTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OutgoingOrders_SellerId",
                table: "OutgoingOrders",
                column: "SellerId");

            migrationBuilder.CreateIndex(
                name: "IX_OutgoingOrders_WarehouseId",
                table: "OutgoingOrders",
                column: "WarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutgoingOrderDetails");

            migrationBuilder.DropTable(
                name: "OutgoingOrderInvoices");

            migrationBuilder.DropTable(
                name: "OutgoingOrderPayments");

            migrationBuilder.DropTable(
                name: "OutgoingOrders");
        }
    }
}

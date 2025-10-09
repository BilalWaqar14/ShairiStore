using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShairiStore.Migrations
{
    /// <inheritdoc />
    public partial class MadeOrderInvoiceOptionalInOrderDetailsModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "OrderDetails",
                keyColumn: "UpdatedBy",
                keyValue: null,
                column: "UpdatedBy",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "OrderDetails",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedBy",
                table: "OrderDetails",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}

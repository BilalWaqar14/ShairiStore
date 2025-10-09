using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShairiStore.Migrations
{
    /// <inheritdoc />
    public partial class AddNavigationalPropertyAndTitleInNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NotificationGeneratedBy",
                table: "NotificationDetails",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "NotificationTitle",
                table: "NotificationDetails",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationDetails_NotificationGeneratedBy",
                table: "NotificationDetails",
                column: "NotificationGeneratedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationDetails_AspNetUsers_NotificationGeneratedBy",
                table: "NotificationDetails",
                column: "NotificationGeneratedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotificationDetails_AspNetUsers_NotificationGeneratedBy",
                table: "NotificationDetails");

            migrationBuilder.DropIndex(
                name: "IX_NotificationDetails_NotificationGeneratedBy",
                table: "NotificationDetails");

            migrationBuilder.DropColumn(
                name: "NotificationTitle",
                table: "NotificationDetails");

            migrationBuilder.AlterColumn<string>(
                name: "NotificationGeneratedBy",
                table: "NotificationDetails",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}

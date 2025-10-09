using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShairiStore.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationModuleInNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NotificationModule",
                table: "NotificationDetails",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotificationModule",
                table: "NotificationDetails");
        }
    }
}

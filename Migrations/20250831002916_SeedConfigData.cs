using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ShairiStore.Migrations
{
    /// <inheritdoc />
    public partial class SeedConfigData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Brands",
                columns: new[] { "BrandId", "BrandName", "IsActive" },
                values: new object[,]
                {
                    { 1, "Zarafa", true },
                    { 2, "Golden Grain", true },
                    { 3, "Shan Foods", true },
                    { 4, "Rafi Daal Factory", true },
                    { 5, "Baison A", true },
                    { 6, "Baison B", true },
                    { 7, "Himalayan Chef", true },
                    { 8, "Nimcos", true },
                    { 9, "Sunridge", true },
                    { 10, "Bake Parlor", true }
                });

            migrationBuilder.InsertData(
                table: "Brokers",
                columns: new[] { "BrokerId", "BrokerCommission", "BrokerName", "IsActive" },
                values: new object[,]
                {
                    { 1, 2.5, "Kamal Subhani", true },
                    { 2, 2.5, "Ali Akbar", true },
                    { 3, 2.5, "Amjad Hussain", true },
                    { 4, 2.5, "Nasir Ali", true }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "CategoryName", "IsActive" },
                values: new object[,]
                {
                    { 1, "Daal", true },
                    { 2, "Baison", true },
                    { 3, "Chawaal", true },
                    { 4, "Channay", true },
                    { 5, "Attah", true }
                });

            migrationBuilder.InsertData(
                table: "OrderStatus",
                columns: new[] { "StatusId", "IsActive", "Status" },
                values: new object[,]
                {
                    { 1, true, "Placed" },
                    { 2, true, "Confirmed" },
                    { 3, true, "Processing" },
                    { 4, true, "Shipped" },
                    { 5, true, "Delivered" },
                    { 6, true, "Rejected" },
                    { 7, true, "OnHold" },
                    { 8, true, "Cancelled" }
                });

            migrationBuilder.InsertData(
                table: "OrderTypes",
                columns: new[] { "OrderTypeId", "IsActive", "TypeName" },
                values: new object[,]
                {
                    { 1, true, "Incoming Order" },
                    { 2, true, "Outgoing Order" }
                });

            migrationBuilder.InsertData(
                table: "Sellers",
                columns: new[] { "SellerId", "IsActive", "SellerName" },
                values: new object[,]
                {
                    { 1, true, "Bilal Waqar" },
                    { 2, true, "Ahmad Waqar" },
                    { 3, true, "Haris Akram" },
                    { 4, true, "Hafiz Asif Akbar" }
                });

            migrationBuilder.InsertData(
                table: "Warehouses",
                columns: new[] { "WarehouseId", "IsActive", "WarehouseAddress", "WarehouseName" },
                values: new object[,]
                {
                    { 1, false, "Usman Shairi Home lahore", "Dehli Gate Warehouse" },
                    { 2, false, "Akbari mandi lahore", "Akbari Mandi Shop" }
                });

            migrationBuilder.InsertData(
                table: "SubCategories",
                columns: new[] { "SubCategoryId", "BrandId", "CategoryId", "IsActive", "OneMonRate", "SubCategoryName" },
                values: new object[,]
                {
                    { 1, 3, 1, true, 100.0, "Daal Mash" },
                    { 2, 4, 1, true, 100.0, "Daal Mash" },
                    { 3, 3, 1, true, 150.0, "Daal Channa" },
                    { 4, 4, 1, true, 500.0, "Daal Channa" },
                    { 5, 5, 2, true, 100.0, "Baison Sub Category 1" },
                    { 6, 6, 2, true, 100.0, "Baison Sub Category 1" },
                    { 7, 5, 2, true, 150.0, "Baison Sub Category 2" },
                    { 8, 6, 2, true, 500.0, "Baison Sub Category 2" },
                    { 9, 1, 3, true, 100.0, "Basmati Rice Premium" },
                    { 10, 2, 3, true, 100.0, "Basmati Rice Premium" },
                    { 11, 1, 3, true, 150.0, "Basmati Rice Average" },
                    { 12, 2, 3, true, 500.0, "Basmati Rice Average" },
                    { 13, 7, 4, true, 100.0, "Safaid Channy" },
                    { 14, 8, 4, true, 100.0, "Safaid Channy" },
                    { 15, 7, 4, true, 150.0, "kaalay Channy" },
                    { 16, 8, 4, true, 500.0, "Kaalay Channy" },
                    { 17, 9, 5, true, 100.0, "Saifaid Attah Premium" },
                    { 18, 10, 5, true, 100.0, "Saifaid Attah Premium" },
                    { 19, 9, 5, true, 150.0, "Mix Attah Premium" },
                    { 20, 10, 5, true, 500.0, "Mix Attah Premium" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Brokers",
                keyColumn: "BrokerId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Brokers",
                keyColumn: "BrokerId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Brokers",
                keyColumn: "BrokerId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Brokers",
                keyColumn: "BrokerId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "OrderStatus",
                keyColumn: "StatusId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "OrderStatus",
                keyColumn: "StatusId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "OrderStatus",
                keyColumn: "StatusId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "OrderStatus",
                keyColumn: "StatusId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "OrderStatus",
                keyColumn: "StatusId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "OrderStatus",
                keyColumn: "StatusId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "OrderStatus",
                keyColumn: "StatusId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "OrderStatus",
                keyColumn: "StatusId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "OrderTypes",
                keyColumn: "OrderTypeId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "OrderTypes",
                keyColumn: "OrderTypeId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Sellers",
                keyColumn: "SellerId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Sellers",
                keyColumn: "SellerId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Sellers",
                keyColumn: "SellerId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Sellers",
                keyColumn: "SellerId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryId",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryId",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryId",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "SubCategoryId",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Warehouses",
                keyColumn: "WarehouseId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Warehouses",
                keyColumn: "WarehouseId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "BrandId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "BrandId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "BrandId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "BrandId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "BrandId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "BrandId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "BrandId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "BrandId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "BrandId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "BrandId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 5);
        }
    }
}

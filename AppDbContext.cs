using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShairiStore.Models;
using System.Reflection.Emit;

namespace ShairiStore;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    // DbSets for your app data here
    public DbSet<Warehouse> Warehouses { get; set; }
    public DbSet<OrderType> OrderTypes { get; set; }
    public DbSet<SellerInfo> Sellers { get; set; }
    public DbSet<BrokerInfo> Brokers { get; set; }
    public DbSet<OrderCategory> Categories { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<OrderSubCategory> SubCategories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }
    public DbSet<PaymentMethod> PaymentMethods { get; set; }
    public DbSet<InvoiceStatus> InvoiceStatuses { get; set; }
    public DbSet<OrderInvoice> OrderInvoices { get; set; }
    public DbSet<NotificationType> NotificationTypes { get; set; }
    public DbSet<NotificationDetails> NotificationDetails { get; set; }
    public DbSet<OrderPayment> OrderPayments { get; set; }
    public DbSet<OrderStatus> OrderStatuses { get; set; }
    public DbSet<Inventory> inventories { get; set; }
    public DbSet<Expense> Expenses { get; set; }
    public DbSet<ExpenseType> ExpenseTypes { get; set; }
    public DbSet<OutgoingOrder> OutgoingOrders { get; set; }
    public DbSet<OutgoingOrderDetails> OutgoingOrderDetails { get; set; }
    public DbSet<OutgoingOrderInvoice> OutgoingOrderInvoices { get; set; }
    public DbSet<OutgoingOrderPayment> OutgoingOrderPayments { get; set; }
    public DbSet<ExpenseStatus> ExpenseStatuses { get; set; }
    public DbSet<CreditPayments> CreditPayments { get; set; }



    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Seed Warehouse
        builder.Entity<Warehouse>().HasData(
            new Warehouse { WarehouseId = 1, WarehouseName = "Dehli Gate Warehouse", WarehouseAddress = "Usman Shairi Home lahore" },
            new Warehouse { WarehouseId = 2, WarehouseName = "Akbari Mandi Shop", WarehouseAddress = "Akbari mandi lahore" }
        );

        // Seed OrderType
        builder.Entity<OrderType>().HasData(
            new OrderType { OrderTypeId = 1, TypeName = "Incoming Order", IsActive = true },
            new OrderType { OrderTypeId = 2, TypeName = "Outgoing Order", IsActive = true }
        );

        // Seed OrderStatus
        builder.Entity<OrderStatus>().HasData(
            new OrderStatus { StatusId = 1, Status = "Placed", IsActive = true },
            new OrderStatus { StatusId = 2, Status = "Confirmed", IsActive = true },
            new OrderStatus { StatusId = 3, Status = "Processing", IsActive = true },
            new OrderStatus { StatusId = 4, Status = "Shipped", IsActive = true },
            new OrderStatus { StatusId = 5, Status = "Delivered", IsActive = true },
            new OrderStatus { StatusId = 6, Status = "Rejected", IsActive = true },
            new OrderStatus { StatusId = 7, Status = "OnHold", IsActive = true },
            new OrderStatus { StatusId = 8, Status = "Cancelled", IsActive = true }
        );

        // Seed SellerInfo
        builder.Entity<SellerInfo>().HasData(
            new SellerInfo { SellerId = 1, SellerName = "Bilal Waqar", IsActive = true },
            new SellerInfo { SellerId = 2, SellerName = "Ahmad Waqar", IsActive = true },
            new SellerInfo { SellerId = 3, SellerName = "Haris Akram", IsActive = true },
            new SellerInfo { SellerId = 4, SellerName = "Hafiz Asif Akbar", IsActive = true }
        );

        // Seed BrokerInfo
        builder.Entity<BrokerInfo>().HasData(
            new BrokerInfo { BrokerId = 1, BrokerName = "Kamal Subhani", BrokerCommission = 2.5, IsActive = true },
            new BrokerInfo { BrokerId = 2, BrokerName = "Ali Akbar", BrokerCommission = 2.5, IsActive = true },
            new BrokerInfo { BrokerId = 3, BrokerName = "Amjad Hussain", BrokerCommission = 2.5, IsActive = true },
            new BrokerInfo { BrokerId = 4, BrokerName = "Nasir Ali", BrokerCommission = 2.5, IsActive = true }
        );

        // Seed OrderCategory
        builder.Entity<OrderCategory>().HasData(
            new OrderCategory { CategoryId = 1, CategoryName = "Daal", IsActive = true },
            new OrderCategory { CategoryId = 2, CategoryName = "Baison", IsActive = true },
            new OrderCategory { CategoryId = 3, CategoryName = "Chawaal", IsActive = true },
            new OrderCategory { CategoryId = 4, CategoryName = "Channay", IsActive = true },
            new OrderCategory { CategoryId = 5, CategoryName = "Attah", IsActive = true }
        );

        // Seed Brand
        builder.Entity<Brand>().HasData(
            new Brand { BrandId = 1, BrandName = "Zarafa", IsActive = true },
            new Brand { BrandId = 2, BrandName = "Golden Grain", IsActive = true },
            new Brand { BrandId = 3, BrandName = "Shan Foods", IsActive = true },
            new Brand { BrandId = 4, BrandName = "Rafi Daal Factory", IsActive = true },
            new Brand { BrandId = 5, BrandName = "Baison A", IsActive = true },
            new Brand { BrandId = 6, BrandName = "Baison B", IsActive = true },
            new Brand { BrandId = 7, BrandName = "Himalayan Chef", IsActive = true },
            new Brand { BrandId = 8, BrandName = "Nimcos", IsActive = true },
            new Brand { BrandId = 9, BrandName = "Sunridge", IsActive = true },
            new Brand { BrandId = 10, BrandName = "Bake Parlor", IsActive = true }
        );

        // Seed OrderSubCategory
        builder.Entity<OrderSubCategory>().HasData(
            new OrderSubCategory { SubCategoryId = 1, SubCategoryName = "Daal Mash", IsActive = true, CategoryId = 1, BrandId = 3, OneMonRate = 100 },
            new OrderSubCategory { SubCategoryId = 2, SubCategoryName = "Daal Mash", IsActive = true, CategoryId = 1, BrandId = 4, OneMonRate = 100 },
            new OrderSubCategory { SubCategoryId = 3, SubCategoryName = "Daal Channa", IsActive = true, CategoryId = 1, BrandId = 3, OneMonRate = 150 },
            new OrderSubCategory { SubCategoryId = 4, SubCategoryName = "Daal Channa", IsActive = true, CategoryId = 1, BrandId = 4, OneMonRate = 500 },
            new OrderSubCategory { SubCategoryId = 5, SubCategoryName = "Baison Sub Category 1", IsActive = true, CategoryId = 2, BrandId = 5, OneMonRate = 100 },
            new OrderSubCategory { SubCategoryId = 6, SubCategoryName = "Baison Sub Category 1", IsActive = true, CategoryId = 2, BrandId = 6, OneMonRate = 100 },
            new OrderSubCategory { SubCategoryId = 7, SubCategoryName = "Baison Sub Category 2", IsActive = true, CategoryId = 2, BrandId = 5, OneMonRate = 150 },
            new OrderSubCategory { SubCategoryId = 8, SubCategoryName = "Baison Sub Category 2", IsActive = true, CategoryId = 2, BrandId = 6, OneMonRate = 500 },
            new OrderSubCategory { SubCategoryId = 9, SubCategoryName = "Basmati Rice Premium", IsActive = true, CategoryId = 3, BrandId = 1, OneMonRate = 100 },
            new OrderSubCategory { SubCategoryId = 10, SubCategoryName = "Basmati Rice Premium", IsActive = true, CategoryId = 3, BrandId = 2, OneMonRate = 100 },
            new OrderSubCategory { SubCategoryId = 11, SubCategoryName = "Basmati Rice Average", IsActive = true, CategoryId = 3, BrandId = 1, OneMonRate = 150 },
            new OrderSubCategory { SubCategoryId = 12, SubCategoryName = "Basmati Rice Average", IsActive = true, CategoryId = 3, BrandId = 2, OneMonRate = 500 },
            new OrderSubCategory { SubCategoryId = 13, SubCategoryName = "Safaid Channy", IsActive = true, CategoryId = 4, BrandId = 7, OneMonRate = 100 },
            new OrderSubCategory { SubCategoryId = 14, SubCategoryName = "Safaid Channy", IsActive = true, CategoryId = 4, BrandId = 8, OneMonRate = 100 },
            new OrderSubCategory { SubCategoryId = 15, SubCategoryName = "kaalay Channy", IsActive = true, CategoryId = 4, BrandId = 7, OneMonRate = 150 },
            new OrderSubCategory { SubCategoryId = 16, SubCategoryName = "Kaalay Channy", IsActive = true, CategoryId = 4, BrandId = 8, OneMonRate = 500 },
            new OrderSubCategory { SubCategoryId = 17, SubCategoryName = "Saifaid Attah Premium", IsActive = true, CategoryId = 5, BrandId = 9, OneMonRate = 100 },
            new OrderSubCategory { SubCategoryId = 18, SubCategoryName = "Saifaid Attah Premium", IsActive = true, CategoryId = 5, BrandId = 10, OneMonRate = 100 },
            new OrderSubCategory { SubCategoryId = 19, SubCategoryName = "Mix Attah Premium", IsActive = true, CategoryId = 5, BrandId = 9, OneMonRate = 150 },
            new OrderSubCategory { SubCategoryId = 20, SubCategoryName = "Mix Attah Premium", IsActive = true, CategoryId = 5, BrandId = 10, OneMonRate = 500 }
        );

        // Seed PaymentMethod
        builder.Entity<PaymentMethod>().HasData(
            new PaymentMethod { PaymentMethodId = 1, PaymentMode = "Cash", IsActive = true },
            new PaymentMethod { PaymentMethodId = 2, PaymentMode = "Online Transfer", IsActive = true },
            new PaymentMethod { PaymentMethodId = 3, PaymentMode = "Bank Checque", IsActive = true }
        );

        // Seed InvoiceStatus
        builder.Entity<InvoiceStatus>().HasData(
            new InvoiceStatus { InvoiceStatusId = 1, StatusName = "Pending", IsActive = true },
            new InvoiceStatus { InvoiceStatusId = 2, StatusName = "Paid", IsActive = true },
            new InvoiceStatus { InvoiceStatusId = 3, StatusName = "Partially Paid", IsActive = true },
            new InvoiceStatus { InvoiceStatusId = 4, StatusName = "Cancelled", IsActive = true }
        );

        // Seed Notification Types
        builder.Entity<NotificationType>().HasData(
            new NotificationType { NotificationTypeId = 1, Type = "User Created", IsActive = true },
            new NotificationType { NotificationTypeId = 2, Type = "User Updated", IsActive = true },
            new NotificationType { NotificationTypeId = 3, Type = "Order Created", IsActive = true },
            new NotificationType { NotificationTypeId = 4, Type = "Order Updated", IsActive = true },
            new NotificationType { NotificationTypeId = 5, Type = "Invoice Created", IsActive = true },
            new NotificationType { NotificationTypeId = 6, Type = "Invoice Paid", IsActive = true },
            new NotificationType { NotificationTypeId = 7, Type = "Payment Received", IsActive = true }
        );

        // Seed Expense Types
        builder.Entity<ExpenseType>().HasData(
            new ExpenseType { ExpenseTypeId = 1, ExpenseName = "Shop Expense", IsActive = true },
            new ExpenseType { ExpenseTypeId = 2, ExpenseName = "Home Expense", IsActive = true },
            new ExpenseType { ExpenseTypeId = 3, ExpenseName = "Transport Expense", IsActive = true },
            new ExpenseType { ExpenseTypeId = 4, ExpenseName = "Electricity Expense", IsActive = true },
            new ExpenseType { ExpenseTypeId = 5, ExpenseName = "Food Expense", IsActive = true },
            new ExpenseType { ExpenseTypeId = 6, ExpenseName = "Credit", IsActive = true },
            new ExpenseType { ExpenseTypeId = 7, ExpenseName = "Others", IsActive = true }
        );


        // Seed ExpenseStatus
        builder.Entity<ExpenseStatus>().HasData(
            new ExpenseStatus { ExpenseStatusId = 1, StatusName = "Pending", IsActive = true },
            new ExpenseStatus { ExpenseStatusId = 2, StatusName = "Paid", IsActive = true }
        );
    }
}

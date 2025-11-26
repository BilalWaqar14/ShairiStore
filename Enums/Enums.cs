namespace ShairiStore.Enums;

public enum ExportTypes
{
    OrderWithDetails = 1,
    Payments = 2,
    Invoices = 3,
    Orders = 4,
    Expenses = 5,
    Credits = 6,
    OutGoingOrders = 7,
    OutgoingOrderDetails = 8
}

public enum ExpenseTypes
{
    ShopExpense = 1,
    HomeExpense = 2,
    TransportExpense = 3,
    ElectricityExpense = 4,
    FoodExpense = 5,
    Credit = 6,
    Others = 7
}

public enum Order_Types
{
    Incoming = 1,
    Outgoing = 2
}

public enum DashboardTypes
{
    IncomingOrder = 1,
    OutgoingOrder,
    Invoices,
    Payments,
    Expenses,
    Credits
}
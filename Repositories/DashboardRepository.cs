using Microsoft.EntityFrameworkCore;
using ShairiStore.Enums;
using ShairiStore.Models;

namespace ShairiStore.Repositories;

public class DashboardRepository : IDashboardRepository
{
    private readonly AppDbContext _context;

    public DashboardRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<DashboardResponse> GetDashboardDataAsync(DashboardRequest request)
    {
        var dashboard = new DashboardResponse();
        bool flag = true;
        // Compute counts in a single query
        dashboard.OutgoingOrderInfo = await _context.OutgoingOrders.Where(x=> x.OrderDate <= request.EndDate && x.OrderDate >= request.StartDate)
            .GroupBy(o => 1)
            .Select(g => new DashboardCard
            {
                TotalCount = g.Count(o => o.OutgoingOrderDetails.Any(d => d.OrderStatusId == 2 || d.OrderStatusId == 4 || d.OrderStatusId == 5 || d.OrderStatusId == 3)),
                PendingCount = g.Count(o => o.OutgoingOrderDetails.Any(d => d.OrderStatusId == 3)),
                ReceivedCount = g.Count(o => o.OutgoingOrderDetails.Any(d => d.OrderStatusId == 5 || d.OrderStatusId == 4)) // 5
            })
            .FirstOrDefaultAsync() ?? new DashboardCard(); // fallback if null

        // Compute counts in a single query
        dashboard.IncomingOrderInfo = await _context.Orders.Where(x => x.OrderDate <= request.EndDate && x.OrderDate >= request.StartDate)
            .GroupBy(o => 1)
            .Select(g => new DashboardCard
            {
                TotalCount = g.Count(o => o.OrderDetails.Any(d => d.OrderStatusId == 1 || d.OrderStatusId == 3 || d.OrderStatusId == 5)),
                PendingCount = g.Count(o => o.OrderDetails.Any(d => d.OrderStatusId == 3)),
                ReceivedCount = g.Count(o => o.OrderDetails.Any(d => d.OrderStatusId == 5))
            })
            .FirstOrDefaultAsync() ?? new DashboardCard(); // fallback if null

        // Compute counts in a single query
        var incomingPayments = await _context.OrderPayments.Where(x => x.UpdatedAt <= request.EndDate && x.UpdatedAt >= request.StartDate)
            .GroupBy(p => 1)
            .Select(g => new DashboardCard
            {
                TotalCount = g.Count(p => p.InvoiceStatusId == 1 || p.InvoiceStatusId == 2 || p.InvoiceStatusId == 3),
                PendingCount = g.Count(p => p.InvoiceStatusId == 1),
                ReceivedCount = g.Count(p => p.InvoiceStatusId == 2)
            })
            .FirstOrDefaultAsync() ?? new DashboardCard();

        var outgoingPayments = await _context.OutgoingOrderPayments.Where(x => x.UpdatedAt <= request.EndDate && x.UpdatedAt >= request.StartDate)
            .GroupBy(p => 1)
            .Select(g => new DashboardCard
            {
                TotalCount = g.Count(p => p.InvoiceStatusId == 1 || p.InvoiceStatusId == 2 || p.InvoiceStatusId == 3),
                PendingCount = g.Count(p => p.InvoiceStatusId == 1),
                ReceivedCount = g.Count(p => p.InvoiceStatusId == 2)
            })
            .FirstOrDefaultAsync() ?? new DashboardCard();

        dashboard.PaymentInfo = new DashboardCard
        {
            TotalCount = incomingPayments.TotalCount + outgoingPayments.TotalCount,
            PendingCount = incomingPayments.PendingCount + outgoingPayments.PendingCount,
            ReceivedCount = incomingPayments.ReceivedCount + outgoingPayments.ReceivedCount,
        };

        // Compute counts in a single query
        var incomingInvoices = await _context.OrderInvoices.Where(x => x.UpdatedAt <= request.EndDate && x.UpdatedAt >= request.StartDate)
            .GroupBy(p => 1)
            .Select(g => new DashboardCard
            {
                TotalCount = g.Count(p => p.InvoiceStatusId == 1 || p.InvoiceStatusId == 2 || p.InvoiceStatusId == 3),
                PendingCount = g.Count(p => p.InvoiceStatusId == 1),
                ReceivedCount = g.Count(p => p.InvoiceStatusId == 2)
            })
            .FirstOrDefaultAsync() ?? new DashboardCard();

        var outgoingInvoices = await _context.OutgoingOrderInvoices.Where(x => x.UpdatedAt <= request.EndDate && x.UpdatedAt >= request.StartDate)
            .GroupBy(p => 1)
            .Select(g => new DashboardCard
            {
                TotalCount = g.Count(p => p.InvoiceStatusId == 1 || p.InvoiceStatusId == 2 || p.InvoiceStatusId == 3),
                PendingCount = g.Count(p => p.InvoiceStatusId == 1),
                ReceivedCount = g.Count(p => p.InvoiceStatusId == 2)
            })
            .FirstOrDefaultAsync() ?? new DashboardCard();

        dashboard.InvoiceInfo = new DashboardCard
        {
            TotalCount = incomingInvoices.TotalCount + outgoingInvoices.TotalCount,
            PendingCount = incomingInvoices.PendingCount + outgoingInvoices.PendingCount,
            ReceivedCount = incomingInvoices.ReceivedCount + outgoingInvoices.ReceivedCount,
        };

        // Compute counts in a single query
        dashboard.CreditInfo = await _context.Expenses.Where(x => x.UpdatedOn <= request.EndDate && x.UpdatedOn >= request.StartDate)
            .Where(x=> x.ExpenseTypeId == (int)ExpenseTypes.Credit)
            .GroupBy(p => 1)
            .Select(g => new DashboardCard
            {
                TotalCount = g.Count(p => p.ExpenseStatusId == 1 || p.ExpenseStatusId == 2),
                PendingCount = g.Count(p => p.ExpenseStatusId == 1),
                ReceivedCount = g.Count(p => p.ExpenseStatusId == 2)
            })
            .FirstOrDefaultAsync() ?? new DashboardCard();

        // Compute counts in a single query
        dashboard.ExpenseInfo = await _context.Expenses.Where(x => x.UpdatedOn <= request.EndDate && x.UpdatedOn >= request.StartDate)
            .Where(x => x.ExpenseTypeId != (int)ExpenseTypes.Credit)
            .GroupBy(p => 1)
            .Select(g => new DashboardCard
            {
                TotalCount = g.Count(p => p.ExpenseStatusId == 1 || p.ExpenseStatusId == 2),
                PendingCount = g.Count(p => p.ExpenseStatusId == 1),
                ReceivedCount = g.Count(p => p.ExpenseStatusId == 2)
            })
            .FirstOrDefaultAsync() ?? new DashboardCard();

        if(request.DashboardTypeId == (int)DashboardTypes.Invoices)
        {
            //var PaidAmounts = await _context.OrderInvoices.Where(x=> x.UpdatedAt <= request.EndDate && x.UpdatedAt >=  request.StartDate).Select(x=> x.InvoiceAmount).ToListAsync(); 
            // 12 element arrays
            var monthlyTotalPending = new double[12];
            var monthlyTotalPaid = new double[12];

            // Query incoming + outgoing IN PARALLEL
            var incoming = await _context.OrderInvoices
                .Where(x => x.UpdatedAt >= request.StartDate &&
                            x.UpdatedAt <= request.EndDate &&
                            (x.InvoiceStatusId == 1 || x.InvoiceStatusId == 2))
                .GroupBy(x => new { Month = x.UpdatedAt.Value.Month, Status = x.InvoiceStatusId })
                .Select(g => new { g.Key.Month, g.Key.Status, Total = g.Sum(x => x.InvoiceAmount) })
                .ToListAsync();

            var outgoing = await _context.OutgoingOrderInvoices
                .Where(x => x.UpdatedAt >= request.StartDate &&
                            x.UpdatedAt <= request.EndDate &&
                            (x.InvoiceStatusId == 1 || x.InvoiceStatusId == 2))
                .GroupBy(x => new { Month = x.UpdatedAt.Value.Month, Status = x.InvoiceStatusId })
                .Select(g => new { g.Key.Month, g.Key.Status, Total = g.Sum(x => x.InvoiceAmount) })
                .ToListAsync();

            // Merge INCOMING
            foreach (var item in incoming)
            {
                if (item.Status == 1)      // Pending
                    monthlyTotalPending[item.Month - 1] += item.Total;

                else if (item.Status == 2) // Paid
                    monthlyTotalPaid[item.Month - 1] += item.Total;
            }

            // Merge OUTGOING
            foreach (var item in outgoing)
            {
                if (item.Status == 1)
                    monthlyTotalPending[item.Month - 1] += item.Total;

                else if (item.Status == 2)
                    monthlyTotalPaid[item.Month - 1] += item.Total;
            }

            // Assign result
            dashboard.PendingAmounts = monthlyTotalPending;
            dashboard.PaidAmounts = monthlyTotalPaid;
        }
        else if(request.DashboardTypeId == (int)DashboardTypes.Payments)
        {
            // 12 element arrays
            var monthlyTotalPending = new double[12];
            var monthlyTotalPaid = new double[12];

            // Query incoming + outgoing IN PARALLEL
            var incoming = await _context.OrderPayments
                .Where(x => x.UpdatedAt >= request.StartDate &&
                            x.UpdatedAt <= request.EndDate &&
                            (x.InvoiceStatusId == 1 || x.InvoiceStatusId == 2))
                .GroupBy(x => new { Month = x.UpdatedAt.Value.Month, Status = x.InvoiceStatusId })
                .Select(g => new { g.Key.Month, g.Key.Status, Total = g.Sum(x => x.AmountPaid) })
                .ToListAsync();

            var outgoing = await _context.OutgoingOrderPayments
                .Where(x => x.UpdatedAt >= request.StartDate &&
                            x.UpdatedAt <= request.EndDate &&
                            (x.InvoiceStatusId == 1 || x.InvoiceStatusId == 2))
                .GroupBy(x => new { Month = x.UpdatedAt.Value.Month, Status = x.InvoiceStatusId })
                .Select(g => new { g.Key.Month, g.Key.Status, Total = g.Sum(x => x.AmountPaid) })
                .ToListAsync();

            // Merge INCOMING
            foreach (var item in incoming)
            {
                if (item.Status == 1)      // Pending
                    monthlyTotalPending[item.Month - 1] += item.Total;

                else if (item.Status == 2) // Paid
                    monthlyTotalPaid[item.Month - 1] += item.Total;
            }

            // Merge OUTGOING
            foreach (var item in outgoing)
            {
                if (item.Status == 1)
                    monthlyTotalPending[item.Month - 1] += item.Total;

                else if (item.Status == 2)
                    monthlyTotalPaid[item.Month - 1] += item.Total;
            }

            // Assign result
            dashboard.PendingAmounts = monthlyTotalPending;
            dashboard.PaidAmounts = monthlyTotalPaid;
        }
        else if (request.DashboardTypeId == (int)DashboardTypes.Credits)
        {
            // 12 element arrays
            var monthlyTotalPending = new double[12];
            var monthlyTotalPaid = new double[12];

            // Query incoming + outgoing IN PARALLEL
            var pending = await _context.CreditPayments
                .Where(x => x.UpdatedAt >= request.StartDate &&
                            x.UpdatedAt <= request.EndDate &&
                            (x.InvoiceStatusId == 1))
                .GroupBy(x => new { Month = x.UpdatedAt.Value.Month})
                .Select(g => new { g.Key.Month, Total = g.Sum(x => x.AmountPaid - x.RemainingAmount) })
                .ToListAsync();

            var paid = await _context.CreditPayments
                .Where(x => x.UpdatedAt >= request.StartDate &&
                            x.UpdatedAt <= request.EndDate &&
                            (x.InvoiceStatusId == 2))
                .GroupBy(x => new { Month = x.UpdatedAt.Value.Month})
                .Select(g => new { g.Key.Month, Total = g.Sum(x => x.AmountPaid) })
                .ToListAsync();

            // Merge INCOMING
            foreach (var item in pending)
            {
                monthlyTotalPending[item.Month - 1] += item.Total;
            }

            // Merge OUTGOING
            foreach (var item in paid)
            {
                monthlyTotalPending[item.Month - 1] += item.Total;
            }

            // Assign result
            dashboard.PendingAmounts = monthlyTotalPending;
            dashboard.PaidAmounts = monthlyTotalPaid;
        }
        else if (request.DashboardTypeId == (int)DashboardTypes.Expenses)
        {
            // 12 element arrays
            var monthlyTotalPending = new double[12];
            var monthlyTotalPaid = new double[12];

            // Query incoming + outgoing IN PARALLEL
            var pending = await _context.Expenses
                .Where(x => x.UpdatedOn >= request.StartDate &&
                            x.UpdatedOn <= request.EndDate &&
                            (x.ExpenseStatusId == 1))
                .GroupBy(x => new { Month = x.UpdatedOn.Month })
                .Select(g => new { g.Key.Month, Total = (double)(g.Sum(x => x.RemainingAmount) ?? 0) })
                .ToListAsync();

            var paid = await _context.Expenses
                .Where(x => x.UpdatedOn >= request.StartDate &&
                            x.UpdatedOn <= request.EndDate &&
                            (x.ExpenseStatusId == 2))
                .GroupBy(x => new { Month = x.UpdatedOn.Month })
                .Select(g => new { g.Key.Month, Total = (double)(g.Sum(x => x.AmountPaid) ?? 0) })
                .ToListAsync();

            // Merge INCOMING
            foreach (var item in pending)
            {
                if (item == null)
                {
                    monthlyTotalPending[item.Month - 1] = 0;
                }
                else
                {
                    monthlyTotalPending[item.Month - 1] += item.Total;
                }
            }

            // Merge OUTGOING
            foreach (var item in paid)
            {
                if (item == null)
                {
                    monthlyTotalPaid[item.Month - 1] = 0;
                }
                else
                {
                    monthlyTotalPaid[item.Month - 1] += item.Total;
                }
            }

            // Assign result
            dashboard.PendingAmounts = monthlyTotalPending;
            dashboard.PaidAmounts = monthlyTotalPaid;
        }
        else
        {
            flag = false;
            dashboard.PaidAmounts = new List<double> ();
            dashboard.PendingAmounts = new List<double> ();
        }

        if (flag)
        {
            dashboard.PieChartData = await GetPieChartData(request);
        }

        return dashboard;
    }

    private async Task<IList<double>> GetPieChartData(DashboardRequest request)
    {
        var pieChartData = new double[3];
        if (request.DashboardTypeId == (int)DashboardTypes.Payments)
        {
            // Query incoming + outgoing IN PARALLEL
            var incoming = await _context.OrderPayments
                .Where(x => x.UpdatedAt >= request.StartDate &&
                            x.UpdatedAt <= request.EndDate &&
                            (x.InvoiceStatusId == 1 || x.InvoiceStatusId == 4 || x.InvoiceStatusId == 2))
                .GroupBy(x => new { Month = x.UpdatedAt.Value.Month, Status = x.InvoiceStatusId })
                .Select(g => new { g.Key.Month, g.Key.Status, Total = g.Sum(x => x.AmountPaid) })
                .ToListAsync();

            var outgoing = await _context.OutgoingOrderPayments
                .Where(x => x.UpdatedAt >= request.StartDate &&
                            x.UpdatedAt <= request.EndDate &&
                            (x.InvoiceStatusId == 1 || x.InvoiceStatusId == 4 || x.InvoiceStatusId == 2))
                .GroupBy(x => new { Month = x.UpdatedAt.Value.Month, Status = x.InvoiceStatusId })
                .Select(g => new { g.Key.Month, g.Key.Status, Total = g.Sum(x => x.AmountPaid) })
                .ToListAsync();

            // Merge INCOMING
            foreach (var item in incoming)
            {
                if (item.Status == 1)      // Pending
                    pieChartData[1] += item.Total;

                else if (item.Status == 2) // Paid
                    pieChartData[0] += item.Total;

                else if (item.Status == 4) // Paid
                    pieChartData[2] += item.Total;
            }

            // Merge OUTGOING
            foreach (var item in outgoing)
            {
                if (item.Status == 1)      // Pending
                    pieChartData[1] += item.Total;

                else if (item.Status == 2) // Paid
                    pieChartData[0] += item.Total;

                else if (item.Status == 4) // Paid
                    pieChartData[2] += item.Total;

            }

        }
        else if (request.DashboardTypeId == (int)DashboardTypes.Invoices)
        {
            // Query incoming + outgoing IN PARALLEL
            var incoming = await _context.OrderInvoices
                .Where(x => x.UpdatedAt >= request.StartDate &&
                            x.UpdatedAt <= request.EndDate &&
                            (x.InvoiceStatusId == 1 || x.InvoiceStatusId == 2 || x.InvoiceStatusId == 4))
                .GroupBy(x => new { Month = x.UpdatedAt.Value.Month, Status = x.InvoiceStatusId })
                .Select(g => new { g.Key.Month, g.Key.Status, Total = g.Sum(x => x.InvoiceAmount) })
                .ToListAsync();

            var outgoing = await _context.OutgoingOrderInvoices
                .Where(x => x.UpdatedAt >= request.StartDate &&
                            x.UpdatedAt <= request.EndDate &&
                            (x.InvoiceStatusId == 1 || x.InvoiceStatusId == 2 || x.InvoiceStatusId == 4))
                .GroupBy(x => new { Month = x.UpdatedAt.Value.Month, Status = x.InvoiceStatusId })
                .Select(g => new { g.Key.Month, g.Key.Status, Total = g.Sum(x => x.InvoiceAmount) })
                .ToListAsync();

            // Merge INCOMING
            foreach (var item in incoming)
            {
                if (item.Status == 1)      // Pending
                    pieChartData[1] += item.Total;

                else if (item.Status == 2) // Paid
                    pieChartData[0] += item.Total;

                else if (item.Status == 4) // Paid
                    pieChartData[2] += item.Total;
            }

            // Merge OUTGOING
            foreach (var item in outgoing)
            {
                if (item.Status == 1)      // Pending
                    pieChartData[1] += item.Total;

                else if (item.Status == 2) // Paid
                    pieChartData[0] += item.Total;

                else if (item.Status == 4) // Paid
                    pieChartData[2] += item.Total;
            }

        }
        else if (request.DashboardTypeId == (int)DashboardTypes.Expenses)
        {
            // Query incoming + outgoing IN PARALLEL
            var pending = await _context.Expenses
                .Where(x => x.UpdatedOn >= request.StartDate &&
                            x.UpdatedOn <= request.EndDate &&
                            (x.ExpenseStatusId == 1))
                .GroupBy(x => new { Month = x.UpdatedOn.Month })
                .Select(g => new { g.Key.Month, Total = (double)(g.Sum(x => x.RemainingAmount) ?? 0) })
                .ToListAsync();

            var paid = await _context.Expenses
                .Where(x => x.UpdatedOn >= request.StartDate &&
                            x.UpdatedOn <= request.EndDate &&
                            (x.ExpenseStatusId == 2))
                .GroupBy(x => new { Month = x.UpdatedOn.Month })
                .Select(g => new { g.Key.Month, Total = (double)(g.Sum(x => x.AmountPaid) ?? 0) })
                .ToListAsync();

            // Merge INCOMING
            foreach (var item in pending)
            {
                if (item == null)
                {
                    pieChartData[1] += 0;
                }
                else
                {
                    pieChartData[1] += item.Total;
                }
            }

            // Merge OUTGOING
            foreach (var item in paid)
            {
                if (item == null)
                {
                    pieChartData[0] += 0;
                }
                else
                {
                    pieChartData[0] += item.Total;
                }
            }

            pieChartData[2] = 0;
        }
        else if (request.DashboardTypeId == (int)DashboardTypes.Credits)
        {
            // Query incoming + outgoing IN PARALLEL
            var pending = await _context.CreditPayments
                .Where(x => x.UpdatedAt >= request.StartDate &&
                            x.UpdatedAt <= request.EndDate &&
                            (x.InvoiceStatusId == 1))
                .GroupBy(x => new { Month = x.UpdatedAt.Value.Month })
                .Select(g => new { g.Key.Month, Total = g.Sum(x => x.AmountPaid - x.RemainingAmount) })
                .ToListAsync();

            var paid = await _context.CreditPayments
                .Where(x => x.UpdatedAt >= request.StartDate &&
                            x.UpdatedAt <= request.EndDate &&
                            (x.InvoiceStatusId == 2))
                .GroupBy(x => new { Month = x.UpdatedAt.Value.Month })
                .Select(g => new { g.Key.Month, Total = g.Sum(x => x.AmountPaid) })
                .ToListAsync();

            // Merge INCOMING
            foreach (var item in pending)
            {
                pieChartData[1] += item.Total;
            }

            // Merge OUTGOING
            foreach (var item in paid)
            {
                pieChartData[0] += item.Total;
            }

            pieChartData[2] = 0;
        }
        else
        {
            return new List<double>();
        }

        return pieChartData;
    }
}

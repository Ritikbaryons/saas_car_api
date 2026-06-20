using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Saas_Car_Management.Core.Entities;
using Saas_Car_Management.Core.Interfaces;
using Saas_Car_Management.Infrastructure.Persistence;

namespace Saas_Car_Management.Infrastructure.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly ApplicationDbContext _context;

        public DashboardRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<object> GetSuperAdminDashboardAsync()
        {
            var totalTenants = await _context.Tenants.CountAsync();
            var activeTenants = await _context.Tenants.CountAsync(t => t.IsActive);
            var totalPlans = await _context.Plans.CountAsync();
            var monthlyRevenue = await _context.Tenants
                .Include(t => t.Plan)
                .Where(t => t.IsActive)
                .SumAsync(t => t.Plan.Price);

            return new
            {
                TotalTenants = totalTenants,
                ActiveTenants = activeTenants,
                TotalPlans = totalPlans,
                MonthlyRevenue = monthlyRevenue
            };
        }

        public async Task<object> GetTenantDashboardAsync(int tenantId)
        {
            var totalCars = await _context.Cars.CountAsync(c => c.TenantId == tenantId);
            var availableCars = await _context.Cars.CountAsync(c => c.TenantId == tenantId && c.Status == "Available");
            var carsOnRide = await _context.Cars.CountAsync(c => c.TenantId == tenantId && c.Status == "OnRide");

            var totalDrivers = await _context.Drivers.CountAsync(d => d.TenantId == tenantId);
            var availableDrivers = await _context.Drivers.CountAsync(d => d.TenantId == tenantId && d.Status == "Available");

            var todayBookings = await _context.Bookings.CountAsync(b => b.TenantId == tenantId && b.BookingDate.Date == DateTime.UtcNow.Date);
            var completedBookings = await _context.Bookings.CountAsync(b => b.TenantId == tenantId && b.Status == "Completed");
            var revenue = await _context.Bookings.Where(b => b.TenantId == tenantId && b.Status == "Completed").SumAsync(b => b.TotalAmount);

            var pendingVendorPayments = await _context.Partners
                .Where(p => p.TenantId == tenantId && p.Type == "Vendor")
                .SumAsync(p => p.Balance);

            var marketplaceTransactions = await _context.MarketplaceTransactions
                .CountAsync(t => t.TenantId == tenantId);

            var activeTrips = await _context.Bookings
                .CountAsync(b => b.TenantId == tenantId && b.Status == "InProgress");

            return new
            {
                TotalCars = totalCars,
                AvailableCars = availableCars,
                CarsOnRide = carsOnRide,
                TotalDrivers = totalDrivers,
                AvailableDrivers = availableDrivers,
                TodayBookings = todayBookings,
                CompletedBookings = completedBookings,
                Revenue = revenue,
                PendingVendorPayments = pendingVendorPayments,
                MarketplaceTransactions = marketplaceTransactions,
                ActiveTrips = activeTrips
            };
        }
    }
}

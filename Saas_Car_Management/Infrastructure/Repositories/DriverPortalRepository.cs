using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Saas_Car_Management.Core.DTOs;
using Saas_Car_Management.Core.Interfaces;
using Saas_Car_Management.Infrastructure.Persistence;

namespace Saas_Car_Management.Infrastructure.Repositories
{
    public class DriverPortalRepository : IDriverPortalRepository
    {
        private readonly ApplicationDbContext _context;

        public DriverPortalRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DriverTripDto?> GetTripByTokenAsync(string token)
        {
            var bv = await _context.BookingVehicles
                .IgnoreQueryFilters()
                .Include(b => b.Booking)
                    .ThenInclude(b => b.Customer)
                .Include(b => b.Car)
                .Include(b => b.PartnerVehicle)
                .FirstOrDefaultAsync(b => b.MagicToken == token);

            if (bv == null || bv.Booking == null) return null;

            string carDetails = "Assigned Vehicle";
            if (bv.Car != null) carDetails = $"{bv.Car.Make} {bv.Car.Model} ({bv.Car.PlateNumber})";
            else if (bv.PartnerVehicle != null) carDetails = $"{bv.PartnerVehicle.Make} {bv.PartnerVehicle.Model} ({bv.PartnerVehicle.PlateNumber})";

            return new DriverTripDto
            {
                BookingId = bv.BookingId,
                CustomerName = bv.Booking.Customer?.Name ?? "Unknown",
                CustomerPhone = bv.Booking.Customer?.Phone ?? "Unknown",
                PickupLocation = bv.Booking.PickupLocation,
                DropLocation = bv.Booking.DropLocation,
                ScheduledStart = bv.Booking.ScheduledStart,
                Status = bv.Booking.Status,
                Notes = bv.Booking.Notes,
                CarDetails = carDetails,
                AssignmentStatus = bv.Status,
                BookingVehicleId = bv.Id
            };
        }

        public async Task<bool> StartTripAsync(string token)
        {
            var bv = await _context.BookingVehicles
                .IgnoreQueryFilters()
                .Include(b => b.Booking)
                .FirstOrDefaultAsync(b => b.MagicToken == token);

            if (bv == null || bv.Booking == null) return false;

            bv.Status = "Active";
            bv.ActualStart = DateTime.UtcNow;
            bv.Booking.Status = "InProgress";

            await _context.SaveChangesAsync();
            return true;
        }
    }
}

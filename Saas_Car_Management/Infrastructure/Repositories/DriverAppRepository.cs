using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Saas_Car_Management.Core.DTOs;
using Saas_Car_Management.Core.Entities;
using Saas_Car_Management.Core.Interfaces;
using Saas_Car_Management.Infrastructure.Persistence;

namespace Saas_Car_Management.Infrastructure.Repositories
{
    public class DriverAppRepository : IDriverAppRepository
    {
        private readonly ApplicationDbContext _context;

        public DriverAppRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        private async Task<int?> GetDriverIdAsync(int userId)
        {
            var driver = await _context.Drivers.FirstOrDefaultAsync(d => d.UserId == userId);
            return driver?.Id;
        }

        public async Task<DriverAppHomeDto?> GetHomeDataAsync(int userId)
        {
            var driverId = await GetDriverIdAsync(userId);
            if (driverId == null) return null;

            var today = DateTime.UtcNow.Date;

            var activeTripsCount = await _context.BookingVehicles
                .Where(bv => bv.DriverId == driverId && bv.Status == "Active")
                .CountAsync();

            var completedTripsCount = await _context.BookingVehicles
                .Where(bv => bv.DriverId == driverId && bv.Status == "Completed")
                .CountAsync();

            var attendance = await _context.DriverAttendances
                .FirstOrDefaultAsync(a => a.DriverId == driverId && a.Date.Date == today);

            return new DriverAppHomeDto
            {
                ActiveTrips = activeTripsCount,
                CompletedTrips = completedTripsCount,
                IsCheckedIn = attendance != null && attendance.CheckInTime != null,
                IsCheckedOut = attendance != null && attendance.CheckOutTime != null
            };
        }

        public async Task<IEnumerable<DriverAppLiveRideDto>> GetLiveRidesAsync(int userId)
        {
            var driverId = await GetDriverIdAsync(userId);
            if (driverId == null) return new List<DriverAppLiveRideDto>();

            var liveRides = await _context.BookingVehicles
                .Include(bv => bv.Booking)
                    .ThenInclude(b => b.Customer)
                .Include(bv => bv.Car)
                .Where(bv => bv.DriverId == driverId && (bv.Status == "Assigned" || bv.Status == "Active" || bv.Status == "InProgress"))
                .Select(bv => new DriverAppLiveRideDto
                {
                    BookingId = bv.BookingId,
                    CustomerName = bv.Booking.Customer != null ? bv.Booking.Customer.Name : "Unknown",
                    CustomerPhone = bv.Booking.Customer != null ? bv.Booking.Customer.Phone : "Unknown",
                    Pickup = bv.Booking.PickupLocation,
                    Drop = bv.Booking.DropLocation,
                    ScheduledStart = bv.Booking.ScheduledStart,
                    Status = bv.Status,
                    CarDetails = bv.Car != null ? $"{bv.Car.Make} {bv.Car.Model}" : "Assigned Car",
                    BookingVehicleId = bv.Id,
                    MagicToken = bv.MagicToken
                })
                .ToListAsync();

            return liveRides;
        }

        public async Task<IEnumerable<DriverAppHistoryDto>> GetHistoryRidesAsync(int userId)
        {
            var driverId = await GetDriverIdAsync(userId);
            if (driverId == null) return new List<DriverAppHistoryDto>();

            var history = await _context.BookingVehicles
                .Include(bv => bv.Booking)
                .Where(bv => bv.DriverId == driverId && (bv.Status == "Completed" || bv.Status == "Cancelled"))
                .OrderByDescending(bv => bv.Booking.ScheduledStart)
                .Select(bv => new DriverAppHistoryDto
                {
                    BookingId = bv.BookingId,
                    Pickup = bv.Booking.PickupLocation,
                    Drop = bv.Booking.DropLocation,
                    Date = bv.Booking.ScheduledStart,
                    Status = bv.Status,
                    Fare = bv.Fare
                })
                .Take(20)
                .ToListAsync();

            return history;
        }

        public async Task<DriverPunchResultDto?> PunchAttendanceAsync(int userId)
        {
            var driverId = await GetDriverIdAsync(userId);
            if (driverId == null) return null;

            var today = DateTime.UtcNow.Date;
            var currentTime = DateTime.UtcNow.TimeOfDay;

            var attendance = await _context.DriverAttendances
                .FirstOrDefaultAsync(a => a.DriverId == driverId && a.Date.Date == today);

            if (attendance == null)
            {
                attendance = new DriverAttendance
                {
                    DriverId = driverId.Value,
                    Date = today,
                    CheckInTime = currentTime,
                    Status = "Present"
                };
                _context.DriverAttendances.Add(attendance);
                await _context.SaveChangesAsync();
                return new DriverPunchResultDto { Message = "Punched In Successfully", Time = currentTime };
            }
            else if (attendance.CheckOutTime == null)
            {
                attendance.CheckOutTime = currentTime;
                await _context.SaveChangesAsync();
                return new DriverPunchResultDto { Message = "Punched Out Successfully", Time = currentTime };
            }

            return new DriverPunchResultDto { Message = "Already punched out for today.", Time = currentTime };
        }
    }
}

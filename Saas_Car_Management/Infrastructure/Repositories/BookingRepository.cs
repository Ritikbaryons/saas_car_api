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
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        public BookingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BookingDto>> GetBookingsAsync(int tenantId)
        {
            var bookings = await _context.Bookings
                .Where(b => b.TenantId == tenantId)
                .Include(b => b.Customer)
                .Include(b => b.BookingVehicles)
                .ToListAsync();

            var dtos = new List<BookingDto>();
            foreach (var b in bookings)
            {
                var vehicles = new List<BookingVehicleDto>();
                foreach (var bv in b.BookingVehicles)
                {
                    string? carName = null;
                    if (bv.CarId.HasValue)
                    {
                        var car = await _context.Cars.FindAsync(bv.CarId.Value);
                        carName = car != null ? $"{car.Make} {car.Model}" : null;
                    }

                    string? driverName = null;
                    if (bv.DriverId.HasValue)
                    {
                        var drv = await _context.Drivers.FindAsync(bv.DriverId.Value);
                        driverName = drv != null ? $"{drv.FirstName} {drv.LastName}" : null;
                    }

                    string? partnerVehicleName = null;
                    if (bv.PartnerVehicleId.HasValue)
                    {
                        var pv = await _context.PartnerVehicles.FindAsync(bv.PartnerVehicleId.Value);
                        partnerVehicleName = pv != null ? $"{pv.Make} {pv.Model}" : null;
                    }

                    string? partnerDriverName = null;
                    if (bv.PartnerDriverId.HasValue)
                    {
                        var pd = await _context.PartnerDrivers.FindAsync(bv.PartnerDriverId.Value);
                        partnerDriverName = pd?.Name;
                    }

                    vehicles.Add(new BookingVehicleDto
                    {
                        Id = bv.Id,
                        BookingId = bv.BookingId,
                        CarId = bv.CarId,
                        CarName = carName,
                        DriverId = bv.DriverId,
                        DriverName = driverName,
                        PartnerVehicleId = bv.PartnerVehicleId,
                        PartnerVehicleName = partnerVehicleName,
                        PartnerDriverId = bv.PartnerDriverId,
                        PartnerDriverName = partnerDriverName,
                        AssignmentType = bv.AssignmentType,
                        Status = bv.Status,
                        ActualStart = bv.ActualStart,
                        ActualEnd = bv.ActualEnd,
                        Quantity = bv.Quantity,
                        RateType = bv.RateType,
                        BaseRate = bv.BaseRate,
                        Distance = bv.Distance,
                        Hours = bv.Hours,
                        Fare = bv.Fare
                    });
                }

                dtos.Add(new BookingDto
                {
                    Id = b.Id,
                    CustomerId = b.CustomerId,
                    CustomerName = b.Customer?.Name ?? "Unknown",
                    BookingDate = b.BookingDate,
                    ScheduledStart = b.ScheduledStart,
                    ScheduledEnd = b.ScheduledEnd,
                    Status = b.Status,
                    TotalAmount = b.TotalAmount,
                    Notes = b.Notes,
                    PickupLocation = b.PickupLocation,
                    DropLocation = b.DropLocation,
                    BookingVehicles = vehicles
                });
            }

            return dtos;
        }

        public async Task<BookingDto?> GetBookingByIdAsync(int id, int tenantId)
        {
            var b = await _context.Bookings
                .Where(x => x.Id == id && x.TenantId == tenantId)
                .Include(x => x.Customer)
                .Include(x => x.BookingVehicles)
                .FirstOrDefaultAsync();

            if (b == null) return null;

            var vehicles = new List<BookingVehicleDto>();
            foreach (var bv in b.BookingVehicles)
            {
                string? carName = null;
                if (bv.CarId.HasValue)
                {
                    var car = await _context.Cars.FindAsync(bv.CarId.Value);
                    carName = car != null ? $"{car.Make} {car.Model}" : null;
                }

                string? driverName = null;
                if (bv.DriverId.HasValue)
                {
                    var drv = await _context.Drivers.FindAsync(bv.DriverId.Value);
                    driverName = drv != null ? $"{drv.FirstName} {drv.LastName}" : null;
                }

                string? partnerVehicleName = null;
                if (bv.PartnerVehicleId.HasValue)
                {
                    var pv = await _context.PartnerVehicles.FindAsync(bv.PartnerVehicleId.Value);
                    partnerVehicleName = pv != null ? $"{pv.Make} {pv.Model}" : null;
                }

                string? partnerDriverName = null;
                if (bv.PartnerDriverId.HasValue)
                {
                    var pd = await _context.PartnerDrivers.FindAsync(bv.PartnerDriverId.Value);
                    partnerDriverName = pd?.Name;
                }

                vehicles.Add(new BookingVehicleDto
                {
                    Id = bv.Id,
                    BookingId = bv.BookingId,
                    CarId = bv.CarId,
                    CarName = carName,
                    DriverId = bv.DriverId,
                    DriverName = driverName,
                    PartnerVehicleId = bv.PartnerVehicleId,
                    PartnerVehicleName = partnerVehicleName,
                    PartnerDriverId = bv.PartnerDriverId,
                    PartnerDriverName = partnerDriverName,
                    AssignmentType = bv.AssignmentType,
                    Status = bv.Status,
                    ActualStart = bv.ActualStart,
                    ActualEnd = bv.ActualEnd,
                    Quantity = bv.Quantity,
                    RateType = bv.RateType,
                    BaseRate = bv.BaseRate,
                    Distance = bv.Distance,
                    Hours = bv.Hours,
                    Fare = bv.Fare
                });
            }

            return new BookingDto
            {
                Id = b.Id,
                CustomerId = b.CustomerId,
                CustomerName = b.Customer?.Name ?? "Unknown",
                BookingDate = b.BookingDate,
                ScheduledStart = b.ScheduledStart,
                ScheduledEnd = b.ScheduledEnd,
                Status = b.Status,
                TotalAmount = b.TotalAmount,
                Notes = b.Notes,
                PickupLocation = b.PickupLocation,
                DropLocation = b.DropLocation,
                BookingVehicles = vehicles
            };
        }

        public async Task<BookingDto?> CreateBookingAsync(int tenantId, CreateBookingDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var booking = new Booking
                {
                    TenantId = tenantId,
                    CustomerId = dto.CustomerId,
                    ScheduledStart = dto.ScheduledStart.ToUniversalTime(),
                    ScheduledEnd = dto.ScheduledEnd.ToUniversalTime(),
                    TotalAmount = dto.TotalAmount,
                    Notes = dto.Notes,
                    PickupLocation = dto.PickupLocation,
                    DropLocation = dto.DropLocation,
                    Status = "Assigned"
                };

                await _context.Bookings.AddAsync(booking);
                await _context.SaveChangesAsync();

                var vehiclesList = new List<BookingVehicleDto>();

                foreach (var v in dto.Vehicles)
                {
                    var bv = new BookingVehicle
                    {
                        TenantId = tenantId,
                        BookingId = booking.Id,
                        AssignmentType = v.AssignmentType,
                        Status = "Assigned",
                        Quantity = v.Quantity,
                        RateType = v.RateType,
                        BaseRate = v.BaseRate,
                        Distance = v.Distance,
                        Hours = v.Hours,
                        Fare = v.Fare
                    };

                    if (v.AssignmentType == "Own")
                    {
                        bv.CarId = v.CarId;
                        bv.DriverId = v.DriverId;

                        // Mark own car & driver as OnRide
                        if (v.CarId.HasValue)
                        {
                            var car = await _context.Cars.FindAsync(v.CarId.Value);
                            if (car != null) car.Status = "OnRide";
                        }
                        if (v.DriverId.HasValue)
                        {
                            var driver = await _context.Drivers.FindAsync(v.DriverId.Value);
                            if (driver != null) driver.Status = "Active";
                        }
                    }
                    else if (v.AssignmentType == "Vendor")
                    {
                        bv.PartnerVehicleId = v.PartnerVehicleId;
                        bv.PartnerDriverId = v.PartnerDriverId;

                        // Pay vendor/partner model logic: update partner balance
                        if (v.PartnerId.HasValue)
                        {
                            var partner = await _context.Partners.FindAsync(v.PartnerId.Value);
                            if (partner != null)
                            {
                                partner.Balance += dto.TotalAmount / dto.Vehicles.Count; // Simple cost splitting
                            }
                        }
                    }
                    else if (v.AssignmentType == "Marketplace")
                    {
                        if (v.MarketplaceOfferId.HasValue)
                        {
                            var offer = await _context.MarketplaceOffers
                                .Include(o => o.MarketplaceRequest)
                                .FirstOrDefaultAsync(o => o.Id == v.MarketplaceOfferId.Value);

                            if (offer != null)
                            {
                                offer.Status = "Accepted";
                                // Find or create assignment
                                var assign = new MarketplaceAssignment
                                {
                                    TenantId = tenantId,
                                    MarketplaceOfferId = offer.Id,
                                    ProviderCompanyId = offer.TenantId,
                                    Price = offer.OfferPrice,
                                    Status = "Assigned",
                                    SettlementStatus = "Pending"
                                };
                                await _context.MarketplaceAssignments.AddAsync(assign);
                                await _context.SaveChangesAsync();

                                bv.PartnerVehicleId = null; // Stored differently or custom
                            }
                        }
                    }

                    await _context.BookingVehicles.AddAsync(bv);
                    await _context.SaveChangesAsync();
                }

                // Generate Invoice
                var invoice = new Invoice
                {
                    TenantId = tenantId,
                    BookingId = booking.Id,
                    InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{booking.Id}",
                    IssueDate = DateTime.UtcNow,
                    DueDate = DateTime.UtcNow.AddDays(30),
                    TotalAmount = dto.TotalAmount,
                    TaxAmount = dto.TotalAmount * 0.15m, // 15% VAT
                    PaidAmount = 0,
                    Status = "Unpaid"
                };
                await _context.Invoices.AddAsync(invoice);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return await GetBookingByIdAsync(booking.Id, tenantId);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> StartBookingAsync(int id, int tenantId)
        {
            var b = await _context.Bookings
                .Include(x => x.BookingVehicles)
                .FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId);

            if (b == null) return false;

            b.Status = "InProgress";
            foreach (var bv in b.BookingVehicles)
            {
                bv.Status = "Active";
                bv.ActualStart = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CompleteBookingAsync(int id, int tenantId)
        {
            var b = await _context.Bookings
                .Include(x => x.BookingVehicles)
                .FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId);

            if (b == null) return false;

            b.Status = "Completed";
            foreach (var bv in b.BookingVehicles)
            {
                bv.Status = "Completed";
                bv.ActualEnd = DateTime.UtcNow;

                // Revert own car & driver back to Available
                if (bv.CarId.HasValue)
                {
                    var car = await _context.Cars.FindAsync(bv.CarId.Value);
                    if (car != null) car.Status = "Available";
                }
                if (bv.DriverId.HasValue)
                {
                    var driver = await _context.Drivers.FindAsync(bv.DriverId.Value);
                    if (driver != null) driver.Status = "Available";
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelBookingAsync(int id, int tenantId)
        {
            var b = await _context.Bookings
                .Include(x => x.BookingVehicles)
                .FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId);

            if (b == null) return false;

            b.Status = "Cancelled";
            foreach (var bv in b.BookingVehicles)
            {
                bv.Status = "Cancelled";

                // Revert own car & driver back to Available
                if (bv.CarId.HasValue)
                {
                    var car = await _context.Cars.FindAsync(bv.CarId.Value);
                    if (car != null) car.Status = "Available";
                }
                if (bv.DriverId.HasValue)
                {
                    var driver = await _context.Drivers.FindAsync(bv.DriverId.Value);
                    if (driver != null) driver.Status = "Available";
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}

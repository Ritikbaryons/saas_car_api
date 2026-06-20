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
    public class VendorRepository : IVendorRepository
    {
        private readonly ApplicationDbContext _context;

        public VendorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PartnerDto>> GetVendorsAsync(int tenantId)
        {
            return await _context.Partners
                .Where(p => p.TenantId == tenantId && p.Type == "Vendor")
                .Select(p => new PartnerDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    ContactName = p.ContactName,
                    Email = p.Email,
                    Phone = p.Phone,
                    Address = p.Address,
                    Type = p.Type,
                    Balance = p.Balance,
                    IsActive = p.IsActive
                })
                .ToListAsync();
        }

        public async Task<PartnerDto?> GetVendorByIdAsync(int id, int tenantId)
        {
            var p = await _context.Partners.FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId && x.Type == "Vendor");
            if (p == null) return null;

            return new PartnerDto
            {
                Id = p.Id,
                Name = p.Name,
                ContactName = p.ContactName,
                Email = p.Email,
                Phone = p.Phone,
                Address = p.Address,
                Type = p.Type,
                Balance = p.Balance,
                IsActive = p.IsActive
            };
        }

        public async Task<PartnerDto?> CreateVendorAsync(int tenantId, CreatePartnerDto dto)
        {
            var p = new Partner
            {
                TenantId = tenantId,
                Name = dto.Name,
                ContactName = dto.ContactName,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address,
                Type = "Vendor",
                Balance = 0,
                IsActive = true
            };

            await _context.Partners.AddAsync(p);
            await _context.SaveChangesAsync();

            return new PartnerDto
            {
                Id = p.Id,
                Name = p.Name,
                ContactName = p.ContactName,
                Email = p.Email,
                Phone = p.Phone,
                Address = p.Address,
                Type = p.Type,
                Balance = p.Balance,
                IsActive = p.IsActive
            };
        }

        public async Task<bool> UpdateVendorAsync(int id, int tenantId, CreatePartnerDto dto)
        {
            var p = await _context.Partners.FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId && x.Type == "Vendor");
            if (p == null) return false;

            p.Name = dto.Name;
            p.ContactName = dto.ContactName;
            p.Email = dto.Email;
            p.Phone = dto.Phone;
            p.Address = dto.Address;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteVendorAsync(int id, int tenantId)
        {
            var p = await _context.Partners.FirstOrDefaultAsync(x => x.Id == id && x.TenantId == tenantId && x.Type == "Vendor");
            if (p == null) return false;

            p.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<PartnerVehicleDto>> GetVendorVehiclesAsync(int partnerId, int tenantId)
        {
            return await _context.PartnerVehicles
                .Where(pv => pv.PartnerId == partnerId && pv.TenantId == tenantId)
                .Select(pv => new PartnerVehicleDto
                {
                    Id = pv.Id,
                    PartnerId = pv.PartnerId,
                    Make = pv.Make,
                    Model = pv.Model,
                    Year = pv.Year,
                    PlateNumber = pv.PlateNumber,
                    Color = pv.Color,
                    IsActive = pv.IsActive
                })
                .ToListAsync();
        }

        public async Task<PartnerVehicleDto?> CreateVendorVehicleAsync(int tenantId, CreatePartnerVehicleDto dto)
        {
            var pv = new PartnerVehicle
            {
                TenantId = tenantId,
                PartnerId = dto.PartnerId,
                Make = dto.Make,
                Model = dto.Model,
                Year = dto.Year,
                PlateNumber = dto.PlateNumber,
                Color = dto.Color,
                IsActive = true
            };

            await _context.PartnerVehicles.AddAsync(pv);
            await _context.SaveChangesAsync();

            return new PartnerVehicleDto
            {
                Id = pv.Id,
                PartnerId = pv.PartnerId,
                Make = pv.Make,
                Model = pv.Model,
                Year = pv.Year,
                PlateNumber = pv.PlateNumber,
                Color = pv.Color,
                IsActive = pv.IsActive
            };
        }

        public async Task<IEnumerable<PartnerDriverDto>> GetVendorDriversAsync(int partnerId, int tenantId)
        {
            return await _context.PartnerDrivers
                .Where(pd => pd.PartnerId == partnerId && pd.TenantId == tenantId)
                .Select(pd => new PartnerDriverDto
                {
                    Id = pd.Id,
                    PartnerId = pd.PartnerId,
                    Name = pd.Name,
                    Phone = pd.Phone,
                    LicenseNumber = pd.LicenseNumber,
                    IsActive = pd.IsActive
                })
                .ToListAsync();
        }

        public async Task<CreatePartnerDriverDto?> CreateVendorDriverAsync(int tenantId, CreatePartnerDriverDto dto)
        {
            var pd = new PartnerDriver
            {
                TenantId = tenantId,
                PartnerId = dto.PartnerId,
                Name = dto.Name,
                Phone = dto.Phone,
                LicenseNumber = dto.LicenseNumber,
                IsActive = true
            };

            await _context.PartnerDrivers.AddAsync(pd);
            await _context.SaveChangesAsync();

            return dto;
        }

        public async Task<IEnumerable<VendorPaymentDto>> GetVendorPaymentsAsync(int tenantId)
        {
            return await _context.VendorPayments
                .Where(vp => vp.TenantId == tenantId)
                .Include(vp => vp.Partner)
                .Select(vp => new VendorPaymentDto
                {
                    Id = vp.Id,
                    PartnerId = vp.PartnerId,
                    PartnerName = vp.Partner.Name,
                    Amount = vp.Amount,
                    PaymentDate = vp.PaymentDate,
                    ReferenceNumber = vp.ReferenceNumber,
                    Notes = vp.Notes
                })
                .ToListAsync();
        }

        public async Task<VendorPaymentDto?> RecordPaymentAsync(int tenantId, CreateVendorPaymentDto dto)
        {
            var partner = await _context.Partners.FirstOrDefaultAsync(p => p.Id == dto.PartnerId && p.TenantId == tenantId);
            if (partner == null) return null;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var vp = new VendorPayment
                {
                    TenantId = tenantId,
                    PartnerId = dto.PartnerId,
                    Amount = dto.Amount,
                    PaymentDate = dto.PaymentDate,
                    ReferenceNumber = dto.ReferenceNumber,
                    Notes = dto.Notes
                };

                // Deduct from Vendor balance since we paid them
                partner.Balance -= dto.Amount;

                await _context.VendorPayments.AddAsync(vp);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return new VendorPaymentDto
                {
                    Id = vp.Id,
                    PartnerId = vp.PartnerId,
                    PartnerName = partner.Name,
                    Amount = vp.Amount,
                    PaymentDate = vp.PaymentDate,
                    ReferenceNumber = vp.ReferenceNumber,
                    Notes = vp.Notes
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        async Task<PartnerDriverDto?> IVendorRepository.CreateVendorDriverAsync(int tenantId, CreatePartnerDriverDto dto)
        {
            var res = await CreateVendorDriverAsync(tenantId, dto);
            if (res == null) return null;

            // Find pd
            var pd = await _context.PartnerDrivers.FirstOrDefaultAsync(x => x.PartnerId == dto.PartnerId && x.LicenseNumber == dto.LicenseNumber);
            if (pd == null) return null;

            return new PartnerDriverDto
            {
                Id = pd.Id,
                PartnerId = pd.PartnerId,
                Name = pd.Name,
                Phone = pd.Phone,
                LicenseNumber = pd.LicenseNumber,
                IsActive = pd.IsActive
            };
        }
    }
}

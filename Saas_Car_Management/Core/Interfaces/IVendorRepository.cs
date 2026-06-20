using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Saas_Car_Management.Core.DTOs;

namespace Saas_Car_Management.Core.Interfaces
{
    public interface IVendorRepository
    {
        Task<IEnumerable<PartnerDto>> GetVendorsAsync(int tenantId);
        Task<PartnerDto?> GetVendorByIdAsync(int id, int tenantId);
        Task<PartnerDto?> CreateVendorAsync(int tenantId, CreatePartnerDto dto);
        Task<bool> UpdateVendorAsync(int id, int tenantId, CreatePartnerDto dto);
        Task<bool> DeleteVendorAsync(int id, int tenantId);

        Task<IEnumerable<PartnerVehicleDto>> GetVendorVehiclesAsync(int partnerId, int tenantId);
        Task<PartnerVehicleDto?> CreateVendorVehicleAsync(int tenantId, CreatePartnerVehicleDto dto);

        Task<IEnumerable<PartnerDriverDto>> GetVendorDriversAsync(int partnerId, int tenantId);
        Task<PartnerDriverDto?> CreateVendorDriverAsync(int tenantId, CreatePartnerDriverDto dto);

        Task<IEnumerable<VendorPaymentDto>> GetVendorPaymentsAsync(int tenantId);
        Task<VendorPaymentDto?> RecordPaymentAsync(int tenantId, CreateVendorPaymentDto dto);
    }
}

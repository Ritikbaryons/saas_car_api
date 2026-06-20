using System.Threading.Tasks;

namespace Saas_Car_Management.Core.Interfaces
{
    public interface IDashboardRepository
    {
        Task<object> GetSuperAdminDashboardAsync();
        Task<object> GetTenantDashboardAsync(int tenantId);
    }
}

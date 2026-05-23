using ZeroFrame.Application.DTOS.Admin;

namespace ZeroFrame.Application.Interfaces
{
    public interface IAdminDashboardService
    {
        Task<AdminDashboardGetDto> ObterDashboardAsync();
    }
}

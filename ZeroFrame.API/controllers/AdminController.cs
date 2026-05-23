using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZeroFrame.Application.DTOS.Admin;
using ZeroFrame.Application.Interfaces;

namespace ZeroFrame.API.Controllers
{
    [ApiController]
    [Authorize(Roles = "Administrador")]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminDashboardService _adminDashboardService;

        public AdminController(IAdminDashboardService adminDashboardService)
        {
            _adminDashboardService = adminDashboardService;
        }

        // GET: api/admin/dashboard
        // Consolida indicadores financeiros e operacionais da tela administrativa.
        [HttpGet("dashboard")]
        public async Task<ActionResult<AdminDashboardGetDto>> ObterDashboard()
        {
            var dashboard = await _adminDashboardService.ObterDashboardAsync();
            return Ok(dashboard);
        }
    }
}

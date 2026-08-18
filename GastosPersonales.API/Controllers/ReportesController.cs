using GastosPersonales.Application.DTOs;
using GastosPersonales.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
namespace GastosPersonales.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ReportesController : ControllerBase
    {
        private readonly IReporteService _reporteService;
        public ReportesController(IReporteService reporteService)
        {
            _reporteService = reporteService;
        }
        private int GetUsuarioId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out var id) ? id : 0;
        }
        //Reporte mensual
        [HttpGet("mensual")]
        public async Task<ActionResult<ReporteMensualDto>> GetReporteMensual([FromQuery] int? mes, [FromQuery] int? anio)
        {
            var m = mes ?? DateTime.UtcNow.Month;
            var a = anio ?? DateTime.UtcNow.Year;
            var reporte = await _reporteService.ObtenerReporteMensualAsync(GetUsuarioId(), m, a);
            return Ok(reporte);
        }
        //Alertas de presupuesto
        [HttpGet("alertas-presupuesto")]
        public async Task<ActionResult> GetAlertasPresupuesto([FromQuery] int? mes, [FromQuery] int? anio)
        {
            var m = mes ?? DateTime.UtcNow.Month;
            var a = anio ?? DateTime.UtcNow.Year;
            var alertas = await _reporteService.ObtenerAlertasPresupuestoAsync(GetUsuarioId(), m, a);
            return Ok(alertas);
        }
        // Exportacion excel
        [HttpGet("exportar/excel")]
        public async Task<IActionResult> ExportarExcel([FromQuery] int? mes, [FromQuery] int? anio)
        {
            var m = mes ?? DateTime.UtcNow.Month;
            var a = anio ?? DateTime.UtcNow.Year;
            var bytes = await _reporteService.ExportarReporteExcelAsync(GetUsuarioId(), m, a);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"ReporteGastos_{m}_{a}.xlsx");
        }
        // Exportacion txt
        [HttpGet("exportar/txt")]
        public async Task<IActionResult> ExportarTxt([FromQuery] int? mes, [FromQuery] int? anio)
        {
            var m = mes ?? DateTime.UtcNow.Month;
            var a = anio ?? DateTime.UtcNow.Year;
            var txt = await _reporteService.ExportarReporteTxtAsync(GetUsuarioId(), m, a);
            var bytes = Encoding.UTF8.GetBytes(txt);
            return File(bytes, "text/plain", $"ReporteGastos_{m}_{a}.txt");
        }
        // Exportacion a JSON
        [HttpGet("exportar/json")]
        public async Task<IActionResult> ExportarJson([FromQuery] int? mes, [FromQuery] int? anio)
        {
            var m = mes ?? DateTime.UtcNow.Month;
            var a = anio ?? DateTime.UtcNow.Year;
            var json = await _reporteService.ExportarReporteJsonAsync(GetUsuarioId(), m, a);
            var bytes = Encoding.UTF8.GetBytes(json);
            return File(bytes, "application/json", $"ReporteGastos_{m}_{a}.json");
        }
    }
}
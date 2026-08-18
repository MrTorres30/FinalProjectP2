using GastosPersonales.Application.DTOs;
using GastosPersonales.Application.ExportStrategies;
using GastosPersonales.Application.Services;
using GastosPersonales.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
namespace GastosPersonales.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ReportesController : ControllerBase
    {
        private readonly IReporteService _reporteService;
        private readonly IGastoRepository _gastoRepository;
        private readonly IExportStrategyFactory _exportFactory;
        public ReportesController(
            IReporteService reporteService,
            IGastoRepository gastoRepository,
            IExportStrategyFactory exportFactory)
        {
            _reporteService = reporteService;
            _gastoRepository = gastoRepository;
            _exportFactory = exportFactory;
        }
        private int GetUsuarioId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out var id) ? id : 0;
        }
        [HttpGet("mensual")]
        public async Task<ActionResult<ReporteMensualDto>> GetReporteMensual([FromQuery] int? mes, [FromQuery] int? anio)
        {
            var m = mes ?? DateTime.UtcNow.Month;
            var a = anio ?? DateTime.UtcNow.Year;
            var reporte = await _reporteService.ObtenerReporteMensualAsync(GetUsuarioId(), m, a);
            return Ok(reporte);
        }
        [HttpGet("alertas-presupuesto")]
        public async Task<ActionResult> GetAlertasPresupuesto([FromQuery] int? mes, [FromQuery] int? anio)
        {
            var m = mes ?? DateTime.UtcNow.Month;
            var a = anio ?? DateTime.UtcNow.Year;
            var alertas = await _reporteService.ObtenerAlertasPresupuestoAsync(GetUsuarioId(), m, a);
            return Ok(alertas);
        }
        // Endpoint Polimórfico usando Factory + Strategy
        [HttpGet("exportar/{formato}")]
        public async Task<IActionResult> Exportar([FromRoute] string formato, [FromQuery] int? mes, [FromQuery] int? anio)
        {
            var m = mes ?? DateTime.UtcNow.Month;
            var a = anio ?? DateTime.UtcNow.Year;
            var usuarioId = GetUsuarioId();
            var fechaInicio = new DateTime(a, m, 1);
            var fechaFin = fechaInicio.AddMonths(1).AddDays(-1);
            var gastos = await _gastoRepository.GetFilteredAsync(usuarioId, fechaInicio, fechaFin, null);
            // Obtener la estrategia por la fabrica
            var estrategia = _exportFactory.ObtenerEstrategia(formato);
            // Ejecutar la exportación 
            var archivoBytes = estrategia.Exportar(gastos, m, a);
            var nombreArchivo = $"ReporteGastos_{m}_{a}{estrategia.Extension}";
            return File(archivoBytes, estrategia.ContentType, nombreArchivo);
        }
    }
}
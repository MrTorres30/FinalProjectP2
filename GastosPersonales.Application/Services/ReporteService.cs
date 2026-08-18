using GastosPersonales.Application.DTOs;
using GastosPersonales.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
namespace GastosPersonales.Application.Services
{
    public class ReporteService : IReporteService
    {
        private readonly IGastoRepository _gastoRepository;
        private readonly IPresupuestoRepository _presupuestoRepository;
        private readonly ICategoriaRepository _categoriaRepository;
        public ReporteService(
            IGastoRepository gastoRepository,
            IPresupuestoRepository presupuestoRepository,
            ICategoriaRepository categoriaRepository)
        {
            _gastoRepository = gastoRepository;
            _presupuestoRepository = presupuestoRepository;
            _categoriaRepository = categoriaRepository;
        }
        public async Task<ReporteMensualDto> ObtenerReporteMensualAsync(int usuarioId, int mes, int anio)
        {
            var fechaInicio = new DateTime(anio, mes, 1);
            var fechaFin = fechaInicio.AddMonths(1).AddDays(-1);
            // Obtener gastos de este mes
            var gastos = await _gastoRepository.GetFilteredAsync(usuarioId, fechaInicio, fechaFin, null);
            var totalGastado = gastos.Sum(g => g.Monto);
            // Obtener gastos del mes anterior
            var fechaInicioAnt = fechaInicio.AddMonths(-1);
            var fechaFinAnt = fechaInicio.AddDays(-1);
            var gastosAnt = await _gastoRepository.GetFilteredAsync(usuarioId, fechaInicioAnt, fechaFinAnt, null);
            var totalGastadoAnt = gastosAnt.Sum(g => g.Monto);
            // Calcular diferencia porcentual
            decimal diferenciaPorcentual = 0;
            if (totalGastadoAnt > 0)
            {
                diferenciaPorcentual = ((totalGastado - totalGastadoAnt) / totalGastadoAnt) * 100;
            }
            // Desglose por categoría
            var desglose = gastos
                .GroupBy(g => g.Categoria?.Nombre ?? "Sin Categoría")
                .Select(grp => new CategoriaGastoDto
                {
                    CategoriaNombre = grp.Key,
                    MontoTotal = grp.Sum(g => g.Monto),
                    Porcentaje = totalGastado > 0 ? (grp.Sum(g => g.Monto) / totalGastado) * 100 : 0
                })
                .OrderByDescending(c => c.MontoTotal)
                .ToList();
            // Top 3 categorías
            var top = desglose.Take(3).ToList();
            return new ReporteMensualDto
            {
                TotalGastado = totalGastado,
                TotalGastadoMesAnterior = totalGastadoAnt,
                DiferenciaPorcentual = Math.Round(diferenciaPorcentual, 2),
                DesgloseCategorias = desglose,
                TopCategorias = top
            };
        }
        public async Task<IEnumerable<PresupuestoAlertaDto>> ObtenerAlertasPresupuestoAsync(int usuarioId, int mes, int anio)
        {
            var presupuestos = await _presupuestoRepository.GetByUsuarioIdAsync(usuarioId);
            var presupuestosMes = presupuestos.Where(p => p.Mes == mes && p.Anio == anio).ToList();
            var alertas = new List<PresupuestoAlertaDto>();
            foreach (var p in presupuestosMes)
            {
                var gastado = await _gastoRepository.GetGastoAcumuladoMesAsync(usuarioId, p.CategoriaId, mes, anio);
                var porcentaje = p.MontoLimite > 0 ? (gastado / p.MontoLimite) * 100 : 0;
                string nivel = "Normal";
                if (porcentaje >= 100) nivel = "Excedido";
                else if (porcentaje >= 80) nivel = "Critico";
                else if (porcentaje >= 50) nivel = "Advertencia";
                alertas.Add(new PresupuestoAlertaDto
                {
                    CategoriaId = p.CategoriaId,
                    CategoriaNombre = p.Categoria?.Nombre ?? "Sin Categoría",
                    MontoLimite = p.MontoLimite,
                    MontoGastado = gastado,
                    PorcentajeConsumido = Math.Round(porcentaje, 2),
                    AlertaNivel = nivel
                });
            }
            return alertas;
        }
        public async Task<byte[]> ExportarReporteExcelAsync(int usuarioId, int mes, int anio)
        {
            var fechaInicio = new DateTime(anio, mes, 1);
            var fechaFin = fechaInicio.AddMonths(1).AddDays(-1);
            var gastos = await _gastoRepository.GetFilteredAsync(usuarioId, fechaInicio, fechaFin, null);
            var datos = gastos.Select(g => new
            {
                Fecha = g.Fecha.ToString("yyyy-MM-dd"),
                Descripcion = g.Descripcion ?? "Sin descripción",
                Categoria = g.Categoria?.Nombre ?? "Sin categoría",
                MetodoPago = g.MetodoPago?.Nombre ?? "Sin método",
                Monto = g.Monto
            }).ToList();
            using var ms = new MemoryStream();
            MiniExcelLibs.MiniExcel.SaveAs(ms, datos);
            return ms.ToArray();
        }
        public async Task<string> ExportarReporteTxtAsync(int usuarioId, int mes, int anio)
        {
            var fechaInicio = new DateTime(anio, mes, 1);
            var fechaFin = fechaInicio.AddMonths(1).AddDays(-1);
            var gastos = await _gastoRepository.GetFilteredAsync(usuarioId, fechaInicio, fechaFin, null);
            var total = gastos.Sum(g => g.Monto);
            var sb = new StringBuilder();
            sb.AppendLine("===============================================");
            sb.AppendLine($"         REPORTE DE GASTOS - MES {mes:D2}/{anio}         ");
            sb.AppendLine("===============================================");
            sb.AppendLine($"Fecha Generación: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"Total Gastado: {total:C}");
            sb.AppendLine("-----------------------------------------------");
            sb.AppendLine(string.Format("{0,-12} | {0,-15} | {0,-15} | {0,-10}", "Fecha", "Categoría", "Método Pago", "Monto"));
            sb.AppendLine("-----------------------------------------------");
            foreach (var g in gastos)
            {
                sb.AppendLine(string.Format("{0,-12:yyyy-MM-dd} | {1,-15} | {2,-15} | {3,-10:C}",
                    g.Fecha,
                    g.Categoria?.Nombre ?? "Sin Categoria",
                    g.MetodoPago?.Nombre ?? "Sin Metodo",
                    g.Monto));
            }
            sb.AppendLine("===============================================");
            return sb.ToString();
        }
        public async Task<string> ExportarReporteJsonAsync(int usuarioId, int mes, int anio)
        {
            var fechaInicio = new DateTime(anio, mes, 1);
            var fechaFin = fechaInicio.AddMonths(1).AddDays(-1);
            var gastos = await _gastoRepository.GetFilteredAsync(usuarioId, fechaInicio, fechaFin, null);
            var datos = gastos.Select(g => new
            {
                g.Id,
                Fecha = g.Fecha.ToString("yyyy-MM-dd"),
                g.Descripcion,
                Categoria = g.Categoria?.Nombre,
                MetodoPago = g.MetodoPago?.Nombre,
                g.Monto
            });
            var options = new JsonSerializerOptions { WriteIndented = true };
            return JsonSerializer.Serialize(datos, options);
        }
    }
}
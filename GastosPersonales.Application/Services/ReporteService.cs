using GastosPersonales.Application.DTOs;
using GastosPersonales.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
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
            // Un topsito / 3
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
    }
}
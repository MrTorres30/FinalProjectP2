using GastosPersonales.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace GastosPersonales.Application.Services
{
    public interface IReporteService
    {
        Task<ReporteMensualDto> ObtenerReporteMensualAsync(int usuarioId, int mes, int anio);
        Task<IEnumerable<PresupuestoAlertaDto>> ObtenerAlertasPresupuestoAsync(int usuarioId, int mes, int anio);
    }
}
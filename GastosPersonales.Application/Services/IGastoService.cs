using GastosPersonales.Application.DTOs;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
namespace GastosPersonales.Application.Services
{
    public interface IGastoService
    {
        Task<GastoDto?> GetByIdAsync(int id, int usuarioId);
        Task<IEnumerable<GastoDto>> GetByUsuarioIdAsync(int usuarioId, FiltroGastoDto filtro);
        Task<GastoDto?> CrearAsync(CrearGastoDto dto, int usuarioId);
        Task<bool> ActualizarAsync(int id, CrearGastoDto dto, int usuarioId);
        Task<bool> EliminarAsync(int id, int usuarioId);
        Task<IEnumerable<GastoDto>> ImportarDesdeExcelAsync(Stream stream, int usuarioId);
    }
}
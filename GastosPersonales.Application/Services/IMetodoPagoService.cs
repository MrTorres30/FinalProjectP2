using GastosPersonales.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GastosPersonales.Application.Services
{
    public interface IMetodoPagoService
    {
        Task<MetodoPagoDto?> GetByIdAsync(int id, int usuarioId);
        Task<IEnumerable<MetodoPagoDto>> GetByUsuarioIdAsync(int usuarioId);
        Task<MetodoPagoDto?> CrearAsync(CrearMetodoPagoDto dto, int usuarioId);
        Task<bool> ActualizarAsync(int id, CrearMetodoPagoDto dto, int usuarioId);
        Task<bool> EliminarAsync(int id, int usuarioId);
    }
}


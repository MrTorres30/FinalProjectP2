using GastosPersonales.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace GastosPersonales.Application.Services
{
    public interface IPresupuestoService
    {
        Task<PresupuestoDto?> GetByIdAsync(int id, int usuarioId);
        Task<IEnumerable<PresupuestoDto>> GetByUsuarioIdAsync(int usuarioId);
        Task<PresupuestoDto?> CrearAsync(CrearPresupuestoDto dto, int usuarioId);
        Task<bool> ActualizarAsync(int id, CrearPresupuestoDto dto, int usuarioId);
        Task<bool> EliminarAsync(int id, int usuarioId);
    }
}

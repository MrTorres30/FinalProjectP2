using GastosPersonales.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GastosPersonales.Application.Services
{
    public interface ICategoriaService
    {
        Task<CategoriaDto?> GetByIdAsync(int id, int usuarioId);
        Task<IEnumerable<CategoriaDto>> GetByUsuarioIdAsync(int usuarioId);
        Task<CategoriaDto?> CrearAsync(CrearCategoriaDto dto, int usuarioId);
        Task<bool> ActualizarAsync(int id, CrearCategoriaDto dto, int usuarioId);
        Task<bool> EliminarAsync(int id, int usuarioId);
    }
}

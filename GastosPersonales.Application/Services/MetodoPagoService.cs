using GastosPersonales.Application.DTOs;
using GastosPersonales.Domain.Entities;
using GastosPersonales.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GastosPersonales.Application.Services
{
    public class MetodoPagoService : IMetodoPagoService
    {
        private readonly IMetodoPagoRepository _metodoPagoRepository;
        public MetodoPagoService(IMetodoPagoRepository metodoPagoRepository)
        {
            _metodoPagoRepository = metodoPagoRepository;
        }
        public async Task<MetodoPagoDto?> GetByIdAsync(int id, int usuarioId)
        {
            var metodo = await _metodoPagoRepository.GetByIdAsync(id);
            if (metodo == null || metodo.UsuarioId != usuarioId) return null;
            return new MetodoPagoDto
            {
                Id = metodo.Id,
                Nombre = metodo.Nombre,
                Icono = metodo.Icono,
                EsActivo = metodo.EsActivo
            };
        }
        public async Task<IEnumerable<MetodoPagoDto>> GetByUsuarioIdAsync(int usuarioId)
        {
            var lista = await _metodoPagoRepository.GetUsuarioIdAsync(usuarioId);
            return lista.Select(m => new MetodoPagoDto
            {
                Id = m.Id,
                Nombre = m.Nombre,
                Icono = m.Icono,
                EsActivo = m.EsActivo
            });
        }
        public async Task<MetodoPagoDto?> CrearAsync(CrearMetodoPagoDto dto, int usuarioId)
        {
            // Validar no duplicar nombres
            var existente = await _metodoPagoRepository.GetByNombreAndUsuarioAsync(dto.Nombre, usuarioId);
            if (existente != null) return null;
            var nuevo = new MetodoPago
            {
                Nombre = dto.Nombre,
                Icono = dto.Icono,
                EsActivo = true,
                UsuarioId = usuarioId
            };
            await _metodoPagoRepository.AddAsync(nuevo);
            return new MetodoPagoDto
            {
                Id = nuevo.Id,
                Nombre = nuevo.Nombre,
                Icono = nuevo.Icono,
                EsActivo = nuevo.EsActivo
            };
        }
        public async Task<bool> ActualizarAsync(int id, CrearMetodoPagoDto dto, int usuarioId)
        {
            var metodo = await _metodoPagoRepository.GetByIdAsync(id);
            if (metodo == null || metodo.UsuarioId != usuarioId) return false;
            // Validar duplicado
            var existente = await _metodoPagoRepository.GetByNombreAndUsuarioAsync(dto.Nombre, usuarioId);
            if (existente != null && existente.Id != id) return false;
            metodo.Nombre = dto.Nombre;
            metodo.Icono = dto.Icono;
            await _metodoPagoRepository.UpdateAsync(metodo);
            return true;
        }
        public async Task<bool> EliminarAsync(int id, int usuarioId)
        {
            var metodo = await _metodoPagoRepository.GetByIdAsync(id);
            if (metodo == null || metodo.UsuarioId != usuarioId) return false;
            await _metodoPagoRepository.DeleteAsync(metodo);
            return true;
        }
    }
}
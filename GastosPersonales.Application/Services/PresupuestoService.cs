using GastosPersonales.Application.DTOs;
using GastosPersonales.Domain.Entities;
using GastosPersonales.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace GastosPersonales.Application.Services
{
    public class PresupuestoService : IPresupuestoService
    {
        private readonly IPresupuestoRepository _presupuestoRepository;
        private readonly ICategoriaRepository _categoriaRepository;
        public PresupuestoService(
            IPresupuestoRepository presupuestoRepository,
            ICategoriaRepository categoriaRepository)
        {
            _presupuestoRepository = presupuestoRepository;
            _categoriaRepository = categoriaRepository;
        }
        public async Task<PresupuestoDto?> GetByIdAsync(int id, int usuarioId)
        {
            var presupuesto = await _presupuestoRepository.GetByIdAsync(id);
            if (presupuesto == null || presupuesto.UsuarioId != usuarioId) return null;
            return new PresupuestoDto
            {
                Id = presupuesto.Id,
                MontoLimite = presupuesto.MontoLimite,
                Mes = presupuesto.Mes,
                Anio = presupuesto.Anio,
                CategoriaId = presupuesto.CategoriaId,
                NombreCategoria = presupuesto.Categoria?.Nombre ?? "Categoría Desconocida"
            };
        }
        public async Task<IEnumerable<PresupuestoDto>> GetByUsuarioIdAsync(int usuarioId)
        {
            var lista = await _presupuestoRepository.GetByUsuarioIdAsync(usuarioId);
            return lista.Select(p => new PresupuestoDto
            {
                Id = p.Id,
                MontoLimite = p.MontoLimite,
                Mes = p.Mes,
                Anio = p.Anio,
                CategoriaId = p.CategoriaId,
                NombreCategoria = p.Categoria?.Nombre ?? "Categoría Desconocida"
            });
        }
        public async Task<PresupuestoDto?> CrearAsync(CrearPresupuestoDto dto, int usuarioId)
        {
            var categoria = await _categoriaRepository.GetbyIdAsync(dto.CategoriaId);
            if (categoria == null || categoria.UsuarioId != usuarioId) return null;
            var existente = await _presupuestoRepository.GetByMesAndCategoriaAsync(usuarioId, dto.CategoriaId, dto.Mes, dto.Anio);
            if (existente != null) return null; 
            var nuevo = new Presupuesto
            {
                MontoLimite = dto.MontoLimite,
                Mes = dto.Mes,
                Anio = dto.Anio,
                CategoriaId = dto.CategoriaId,
                UsuarioId = usuarioId
            };
            await _presupuestoRepository.AddAsync(nuevo);
            return new PresupuestoDto
            {
                Id = nuevo.Id,
                MontoLimite = nuevo.MontoLimite,
                Mes = nuevo.Mes,
                Anio = nuevo.Anio,
                CategoriaId = nuevo.CategoriaId,
                NombreCategoria = categoria.Nombre
            };
        }
        public async Task<bool> ActualizarAsync(int id, CrearPresupuestoDto dto, int usuarioId)
        {
            var presupuesto = await _presupuestoRepository.GetByIdAsync(id);
            if (presupuesto == null || presupuesto.UsuarioId != usuarioId) return false;
            // Validar que la categoría nueva exista y pertenezca al usuario
            var categoria = await _categoriaRepository.GetbyIdAsync(dto.CategoriaId);
            if (categoria == null || categoria.UsuarioId != usuarioId) return false;
            // Validar si el nuevo mes/año/categoría choca con otro presupuesto diferente
            var existente = await _presupuestoRepository.GetByMesAndCategoriaAsync(usuarioId, dto.CategoriaId, dto.Mes, dto.Anio);
            if (existente != null && existente.Id != id) return false;
            presupuesto.MontoLimite = dto.MontoLimite;
            presupuesto.Mes = dto.Mes;
            presupuesto.Anio = dto.Anio;
            presupuesto.CategoriaId = dto.CategoriaId;
            await _presupuestoRepository.UpdateAsync(presupuesto);
            return true;
        }
        public async Task<bool> EliminarAsync(int id, int usuarioId)
        {
            var presupuesto = await _presupuestoRepository.GetByIdAsync(id);
            if (presupuesto == null || presupuesto.UsuarioId != usuarioId) return false;
            await _presupuestoRepository.DeleteAsync(presupuesto);
            return true;
        }
    }
}
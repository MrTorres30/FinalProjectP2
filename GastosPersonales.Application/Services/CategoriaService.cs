using GastosPersonales.Application.DTOs;
using GastosPersonales.Domain.Entities;
using GastosPersonales.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GastosPersonales.Application.Services
{

        public class CategoriaService : ICategoriaService
        {
            private readonly ICategoriaRepository _categoriaRepository;
            public CategoriaService(ICategoriaRepository categoriaRepository)
            {
                _categoriaRepository = categoriaRepository;
            }
            public async Task<CategoriaDto?> GetByIdAsync(int id, int usuarioId)
            {
                var categoria = await _categoriaRepository.GetbyIdAsync(id);

                if (categoria == null || categoria.UsuarioId != usuarioId) return null;
                return new CategoriaDto
                {
                    Id = categoria.Id,
                    Nombre = categoria.Nombre,
                    Descripcion = categoria.Descripcion,
                    EsActivo = categoria.EsActivo
                };
            }
            public async Task<IEnumerable<CategoriaDto>> GetByUsuarioIdAsync(int usuarioId)
            {
                var lista = await _categoriaRepository.GetUsuarioIdAsync(usuarioId);
                return lista.Select(c => new CategoriaDto
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    Descripcion = c.Descripcion,
                    EsActivo = c.EsActivo
                });
            }
            public async Task<CategoriaDto?> CrearAsync(CrearCategoriaDto dto, int usuarioId)
            {
                var existente = await _categoriaRepository.GetByNombreAndUsuarioAsync(dto.Nombre, usuarioId);
                if (existente != null) return null;
                var nueva = new Categoria
                {
                    Nombre = dto.Nombre,
                    Descripcion = dto.Descripcion,
                    EsActivo = true,
                    UsuarioId = usuarioId
                };
                await _categoriaRepository.AddAsync(nueva);
                return new CategoriaDto
                {
                    Id = nueva.Id,
                    Nombre = nueva.Nombre,
                    Descripcion = nueva.Descripcion,
                    EsActivo = nueva.EsActivo
                };
            }
            public async Task<bool> ActualizarAsync(int id, CrearCategoriaDto dto, int usuarioId)
            {
                var categoria = await _categoriaRepository.GetbyIdAsync(id);
                if (categoria == null || categoria.UsuarioId != usuarioId) return false;

                var existente = await _categoriaRepository.GetByNombreAndUsuarioAsync(dto.Nombre, usuarioId);

                if (existente != null && existente.Id != id) return false;
                categoria.Nombre = dto.Nombre;
                categoria.Descripcion = dto.Descripcion;
                await _categoriaRepository.UpdateAsync(categoria);
                return true;
            }
            public async Task<bool> EliminarAsync(int id, int usuarioId)
            {
                var categoria = await _categoriaRepository.GetbyIdAsync(id);
                if (categoria == null || categoria.UsuarioId != usuarioId) return false;
                await _categoriaRepository.DeleteAsync(categoria);
                return true;
        }
    }
}
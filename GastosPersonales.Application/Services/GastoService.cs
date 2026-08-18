using GastosPersonales.Application.DTOs;
using GastosPersonales.Domain.Entities;
using GastosPersonales.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
namespace GastosPersonales.Application.Services
{
    public class GastoService : IGastoService
    {
        private readonly IGastoRepository _gastoRepository;
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IMetodoPagoRepository _metodoPagoRepository;
        private readonly IPresupuestoRepository _presupuestoRepository;
        private readonly IExcelService _excelService;
        public GastoService(
            IGastoRepository gastoRepository,
            ICategoriaRepository categoriaRepository,
            IMetodoPagoRepository metodoPagoRepository,
            IPresupuestoRepository presupuestoRepository,
            IExcelService excelService)
        {
            _gastoRepository = gastoRepository;
            _categoriaRepository = categoriaRepository;
            _metodoPagoRepository = metodoPagoRepository;
            _presupuestoRepository = presupuestoRepository;
            _excelService = excelService;
        }
        public async Task<GastoDto?> GetByIdAsync(int id, int usuarioId)
        {
            var gasto = await _gastoRepository.GetByIdAsync(id);
            if (gasto == null || gasto.UsuarioId != usuarioId) return null;
            return MapToDto(gasto, false);
        }
        public async Task<IEnumerable<GastoDto>> GetByUsuarioIdAsync(int usuarioId, FiltroGastoDto filtro)
        {
            var lista = await _gastoRepository.GetFilteredAsync(usuarioId, filtro.FechaInicio, filtro.FechaFin, filtro.CategoriaId);
            return lista.Select(g => MapToDto(g, false));
        }
        public async Task<GastoDto?> CrearAsync(CrearGastoDto dto, int usuarioId)
        {
            var categoria = await _categoriaRepository.GetbyIdAsync(dto.CategoriaId);
            if (categoria == null || categoria.UsuarioId != usuarioId) return null;
            var metodo = await _metodoPagoRepository.GetByIdAsync(dto.MetodoPagoId);
            if (metodo == null || metodo.UsuarioId != usuarioId) return null;
            var nuevoGasto = new Gasto
            {
                Monto = dto.Monto,
                Fecha = dto.Fecha,
                Descripcion = dto.Descripcion,
                CategoriaId = dto.CategoriaId,
                MetodoPagoId = dto.MetodoPagoId,
                UsuarioId = usuarioId
            };
            await _gastoRepository.AddAsync(nuevoGasto);
            bool superoPresupuesto = await VerificarSuperacionPresupuestoAsync(usuarioId, dto.CategoriaId, dto.Fecha, dto.Monto);
            nuevoGasto.Categoria = categoria;
            nuevoGasto.MetodoPago = metodo;
            return MapToDto(nuevoGasto, superoPresupuesto);
        }
        public async Task<bool> ActualizarAsync(int id, CrearGastoDto dto, int usuarioId)
        {
            var gasto = await _gastoRepository.GetByIdAsync(id);
            if (gasto == null || gasto.UsuarioId != usuarioId) return false;
            var categoria = await _categoriaRepository.GetbyIdAsync(dto.CategoriaId);
            if (categoria == null || categoria.UsuarioId != usuarioId) return false;
            var metodo = await _metodoPagoRepository.GetByIdAsync(dto.MetodoPagoId);
            if (metodo == null || metodo.UsuarioId != usuarioId) return false;
            gasto.Monto = dto.Monto;
            gasto.Fecha = dto.Fecha;
            gasto.Descripcion = dto.Descripcion;
            gasto.CategoriaId = dto.CategoriaId;
            gasto.MetodoPagoId = dto.MetodoPagoId;
            await _gastoRepository.UpdateAsync(gasto);
            return true;
        }
        public async Task<bool> EliminarAsync(int id, int usuarioId)
        {
            var gasto = await _gastoRepository.GetByIdAsync(id);
            if (gasto == null || gasto.UsuarioId != usuarioId) return false;
            await _gastoRepository.DeleteAsync(gasto);
            return true;
        }
         public async Task<IEnumerable<GastoDto>> ImportarDesdeExcelAsync(Stream stream, int usuarioId)
        {
            var gastosImportados = await _excelService.LeerGastosDesdeExcelAsync(stream);
            var resultados = new List<GastoDto>();
            var categorias = (await _categoriaRepository.GetUsuarioIdAsync(usuarioId)).ToList();
            var metodos = (await _metodoPagoRepository.GetUsuarioIdAsync(usuarioId)).ToList();
            var catDefecto = categorias.FirstOrDefault();
            var metDefecto = metodos.FirstOrDefault();
            if (catDefecto == null || metDefecto == null) return resultados;
            foreach (var dto in gastosImportados)
            {
                var nuevo = new Gasto
                {
                    Monto = dto.Monto,
                    Fecha = dto.Fecha,
                    Descripcion = string.IsNullOrWhiteSpace(dto.Descripcion) ? "Gasto importado" : dto.Descripcion,
                    CategoriaId = catDefecto.Id,
                    MetodoPagoId = metDefecto.Id,
                    UsuarioId = usuarioId
                };
                await _gastoRepository.AddAsync(nuevo);
                nuevo.Categoria = catDefecto;
                nuevo.MetodoPago = metDefecto;
                resultados.Add(MapToDto(nuevo, false));
            }
            return resultados;
        }
        private GastoDto MapToDto(Gasto gasto, bool superoPresupuesto)
        {
            return new GastoDto
            {
                Id = gasto.Id,
                Monto = gasto.Monto,
                Fecha = gasto.Fecha,
                Descripcion = gasto.Descripcion,
                CategoriaId = gasto.CategoriaId,
                NombreCategoria = gasto.Categoria?.Nombre ?? "Desconocida",
                MetodoPagoId = gasto.MetodoPagoId,
                NombreMetodoPago = gasto.MetodoPago?.Nombre ?? "Desconocido",
                LimitePresupuestoSuperado = superoPresupuesto
            };
        }
        private async Task<bool> VerificarSuperacionPresupuestoAsync(int usuarioId, int categoriaId, DateTime fecha, decimal montoNuevoGasto)
        {
            var presupuesto = await _presupuestoRepository.GetByMesAndCategoriaAsync(usuarioId, categoriaId, fecha.Month, fecha.Year);
            if (presupuesto == null) return false; 
            var totalGastado = await _gastoRepository.GetGastoAcumuladoMesAsync(usuarioId, categoriaId, fecha.Month, fecha.Year);
            return (totalGastado + montoNuevoGasto) > presupuesto.MontoLimite;
        }
    }
}
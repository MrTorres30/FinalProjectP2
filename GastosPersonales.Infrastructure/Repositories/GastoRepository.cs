using GastosPersonales.Domain.Entities;
using GastosPersonales.Domain.Repositories;
using GastosPersonales.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GastosPersonales.Infrastructure.Repositories
{
    public class GastoRepository : IGastoRepository
    {
        private readonly ApplicationDbContext _context;
    
        public GastoRepository(ApplicationDbContext context)
        {
            _context = context;
        }
            public async Task<Gasto?> GetByIdAsync(int id)
        {
            return await _context.Gastos.FindAsync(id);
        }

        public async Task<IEnumerable<Gasto>> GetFilteredAsync(int usuarioId, DateTime? fechaInicio, DateTime? fechaFin, int? categoriaId)
        {
            var query = _context.Gastos
                .Include(g => g.Categoria)
                .Include(g => g.MetodoPago)
                .Where(g => g.UsuarioId == usuarioId)
                .AsQueryable();
            if (fechaInicio.HasValue)
                query = query.Where(g => g.Fecha >= fechaInicio.Value);
            if (fechaFin.HasValue)
                query = query.Where(g => g.Fecha <= fechaFin.Value);
            if (categoriaId.HasValue)
                query = query.Where(g => g.CategoriaId == categoriaId.Value);
            return await query.OrderByDescending(g => g.Fecha).ToListAsync();
        }

        public async Task AddAsync(Gasto gasto)
        {
            await _context.Gastos.AddAsync(gasto);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Gasto gasto)
        {
            _context.Gastos.Update(gasto);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(Gasto gasto)
        {
            _context.Gastos.Remove(gasto);
            await _context.SaveChangesAsync();
        }
        public async Task<decimal> GetGastoAcumuladoMesAsync(int usuarioId, int categoriaId, int mes, int anio)
        {
            return await _context.Gastos
                .Where(g => g.UsuarioId == usuarioId &&
                            g.CategoriaId == categoriaId &&
                            g.Fecha.Month == mes &&
                            g.Fecha.Year == anio)
                .SumAsync(g => g.Monto);
        }
    }
}

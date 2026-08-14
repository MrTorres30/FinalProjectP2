using GastosPersonales.Domain.Entities;
using GastosPersonales.Domain.Repositories;
using GastosPersonales.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace GastosPersonales.Infrastructure.Repositories
{
    public class PresupuestoRepository : IPresupuestoRepository
    {
        private readonly ApplicationDbContext _context;

        public PresupuestoRepository (ApplicationDbContext context)
        {
            _context = context;

        }

        public async Task<Presupuesto?> GetByIdAsync(int id)
        {
            return await _context.Presupuestos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<IEnumerable<Presupuesto>> GetByUsuarioIdAsync(int usuarioId)
        {
            return await _context.Presupuestos
                .Include(p => p.Categoria)
                .Where(p => p.UsuarioId == usuarioId)
                .ToListAsync();
        }
        public async Task<Presupuesto?> GetByMesAndCategoriaAsync(int usuarioId, int categoriaId, int mes, int anio)
        {
            return await _context.Presupuestos
                .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId &&
                                         p.CategoriaId == categoriaId &&
                                         p.Mes == mes &&
                                         p.Anio == anio);
        }
        public async Task AddAsync(Presupuesto presupuesto)
        {
            await _context.Presupuestos.AddAsync(presupuesto);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Presupuesto presupuesto)
        {
            _context.Presupuestos.Update(presupuesto);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(Presupuesto presupuesto)
        {
            _context.Presupuestos.Remove(presupuesto);
            await _context.SaveChangesAsync();
        }
    }
}

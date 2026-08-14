using GastosPersonales.Domain.Entities;
using GastosPersonales.Domain.Repositories;
using GastosPersonales.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GastosPersonales.Infrastructure.Repositories
{
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoriaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Categoria?> GetbyIdAsync(int id)
        {
            return await _context.Categorias.FindAsync(id);
        }
        public async Task<IEnumerable<Categoria>> GetUsuarioIdAsync(int usuarioId)
        {
            return await _context.Categorias
            .Where(c => c.UsuarioId == usuarioId)
            .ToListAsync();
        }
            
        public async Task<Categoria?> GetByNombreAndUsuarioAsync(string nombre, int usuarioId)
        {
            return await _context.Categorias
                .FirstOrDefaultAsync(c => c.Nombre.ToLower() == nombre.ToLower() && c.UsuarioId == usuarioId);
        }

        public async Task AddAsync(Categoria categoria)
        {
            await _context.Categorias.AddAsync(categoria);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Categoria categoria)
        {
             _context.Categorias.Update(categoria);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Categoria categoria)
        {
             _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();
        }
    }
}

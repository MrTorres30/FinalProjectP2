using GastosPersonales.Domain.Entities;
using GastosPersonales.Domain.Repositories;
using GastosPersonales.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GastosPersonales.Infrastructure.Repositories
{
    public class MetodoPagoRepository :IMetodoPagoRepository
    {
        private readonly ApplicationDbContext _context;

        public MetodoPagoRepository(ApplicationDbContext context)
        {

            _context = context;
        }
        public async Task<MetodoPago?> GetByIdAsync(int id)
        {
            return await  _context.MetodosPago.FindAsync(id);
        }

        public async Task<IEnumerable<MetodoPago>> GetUsuarioIdAsync(int usuarioId)
        {
            return await _context.MetodosPago
              .Where (m=> m.UsuarioId == usuarioId)
              .ToListAsync();
        }

        public async Task<MetodoPago?> GetByNombreAndUsuarioAsync (string nombre, int usuarioId)
        {
            return await _context.MetodosPago
                .FirstOrDefaultAsync(m => m.Nombre.ToLower() == nombre.ToLower() && m.UsuarioId == usuarioId);
        }

        public async Task AddAsync (MetodoPago metodoPago)
        {
            await _context.MetodosPago.AddAsync(metodoPago);
               await _context.SaveChangesAsync(); 
       }

        public async Task UpdateAsync (MetodoPago metodoPago)
        {
             _context.MetodosPago.Update(metodoPago);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(MetodoPago metodoPago)
        {
            _context.MetodosPago.Remove(metodoPago);
            await _context.SaveChangesAsync();
        }
    }
}

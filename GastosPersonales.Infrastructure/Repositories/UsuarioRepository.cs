using GastosPersonales.Domain.Entities;
using GastosPersonales.Domain.Repositories;
using GastosPersonales.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace GastosPersonales.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly ApplicationDbContext _context;

        public UsuarioRepository(ApplicationDbContext context) 
        {
            _context = context;
        }

        public async Task<Usuario?> GetUsuarioAsync(int id)
        {
            return await _context.Usuarios.FindAsync(id);
        }

        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task AddAsync(Usuario usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync(); 
        }
    }
}
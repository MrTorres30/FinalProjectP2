using GastosPersonales.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GastosPersonales.Domain.Repositories
{
    public interface ICategoriaRepository
    {
        Task<Categoria?>GetbyIdAsync(int id);
        Task<IEnumerable<Categoria>> GetUsuarioIdAsync(int usuarioId);
        Task<Categoria?> GetByNombreAndUsuarioAsync(string nombre, int usuarioId);
        Task AddAsync(Categoria categoria);
        Task UpdateAsync(Categoria categoria);
        Task DeleteAsync(Categoria categoria);
    }
}

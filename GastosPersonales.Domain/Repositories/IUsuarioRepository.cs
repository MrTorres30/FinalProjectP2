using GastosPersonales.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GastosPersonales.Domain.Repositories
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> GetUsuarioAsync(int id);
        Task<Usuario?> GetByEmailAsync(string email);
        Task AddAsync(Usuario usuario);
        Task UpdateAsync(Usuario usuario);

    }
}

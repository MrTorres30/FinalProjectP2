using GastosPersonales.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GastosPersonales.Domain.Repositories
{
    public interface IMetodoPagoRepository
    {

        Task<MetodoPago?> GetByIDAsync(int id);
        Task <IEnumerable<MetodoPago>> GetUsuarioIdAsync(int usuarioId);
        Task<MetodoPago?> GetByNombreAndUsuarioAsync(string nombre, int usuarioId);
        Task AddAsync (MetodoPago metodoPago);
        Task UpdateAsync(MetodoPago metodopago);
        Task DeleteAsync(MetodoPago metodoPago);
    }
}

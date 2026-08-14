using GastosPersonales.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GastosPersonales.Domain.Repositories
{
    public interface IPresupuestoRepository
    {
        Task<Presupuesto> GetByIdAsync(int id);
        Task<IEnumerable<Presupuesto>> GetByUsuarioIdAsync(int usuarioId);
        Task<Presupuesto?> GetByMesAndCategoriaAsync(int usuarioId, int categoriaId, int mes, int anio); 
        Task AddAsync(Presupuesto presupuesto);
        Task UpdateAsync(Presupuesto presupuesto);
        Task DeleteAsync(Presupuesto presupuesto);
    }
}

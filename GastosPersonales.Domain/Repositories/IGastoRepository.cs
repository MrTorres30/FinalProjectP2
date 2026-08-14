using System;
using System.Collections.Generic;
using System.Text;
using GastosPersonales.Domain.Entities;
using System.Threading.Tasks;

namespace GastosPersonales.Domain.Repositories
{
    public interface IGastoRepository
    {
        Task<Gasto?> GetByIdAsync(int id);
        Task<IEnumerable<Gasto>> GetFilteredAsync(int usuarioId, DateTime? fechaInicio, DateTime? fechaFin, int? categoriaId);
        Task AddAsync(Gasto gasto);
        Task UpdateAsync (Gasto gasto);
        Task DeleteAsync(Gasto gasto);

        Task<decimal> GetGastoAcumuladoMesAsync(int usuarioId, int categoriaId, int mes, int anio);
    }
}

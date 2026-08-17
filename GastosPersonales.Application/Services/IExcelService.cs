using GastosPersonales.Application.DTOs;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace GastosPersonales.Application.Services
{
    public interface IExcelService
    {
        Task<IEnumerable<CrearGastoDto>> LeerGastosDesdeExcelAsync(Stream fileStream);
    }
}


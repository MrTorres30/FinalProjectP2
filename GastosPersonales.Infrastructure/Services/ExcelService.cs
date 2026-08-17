using GastosPersonales.Application.DTOs;
using GastosPersonales.Application.Services;
using MiniExcelLibs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
namespace GastosPersonales.Infrastructure.Services
{
    public class ExcelService : IExcelService
    {
        public async Task<IEnumerable<CrearGastoDto>> LeerGastosDesdeExcelAsync(Stream fileStream)
        {
            var rows = await fileStream.QueryAsync();
            var lista = new List<CrearGastoDto>();
            foreach (var rowObj in rows)
            {
                var row = rowObj as IDictionary<string, object>;
                if (row == null || row.Count < 3) continue;
                string montoStr = GetValue(row, "Monto", "A");
                string fechaStr = GetValue(row, "Fecha", "B");
                string descStr = GetValue(row, "Descripcion", "C");
                if (montoStr.Equals("Monto", StringComparison.OrdinalIgnoreCase)) continue;
                if (decimal.TryParse(montoStr, out decimal monto) &&
                    DateTime.TryParse(fechaStr, out DateTime fecha))
                {
                    lista.Add(new CrearGastoDto
                    {
                        Monto = monto,
                        Fecha = fecha,
                        Descripcion = descStr,
                        CategoriaId = 0, 
                        MetodoPagoId = 0
                    });
                }
            }
            return lista;
        }
        private string GetValue(IDictionary<string, object> row, string key1, string key2)
        {
            if (row.TryGetValue(key1, out var val1) && val1 != null) return val1.ToString() ?? "";
            if (row.TryGetValue(key2, out var val2) && val2 != null) return val2.ToString() ?? "";
            return "";
        }
    }
}
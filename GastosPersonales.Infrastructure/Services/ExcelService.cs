using GastosPersonales.Application.DTOs;
using GastosPersonales.Application.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
namespace GastosPersonales.Infrastructure.Services
{
    public class ExcelService : IExcelService
    {
        public async Task<IEnumerable<CrearGastoDto>> LeerGastosDesdeExcelAsync(Stream fileStream)
        {
            var lista = new List<CrearGastoDto>();
            using var reader = new StreamReader(fileStream);
            string? headerLine = await reader.ReadLineAsync();
            if (headerLine == null) return lista;
            char separator = headerLine.Contains(';') ? ';' : ',';
            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                var cols = line.Split(separator);
                if (cols.Length < 3) continue;
                decimal monto = 0;
                DateTime fecha = DateTime.Now;
                string descripcion = cols[2].Trim();
                if (decimal.TryParse(cols[0].Trim().Replace('$', ' '), NumberStyles.Any, CultureInfo.InvariantCulture, out var m1))
                {
                    monto = m1;
                    DateTime.TryParse(cols[1].Trim(), CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha);
                }
                else if (DateTime.TryParse(cols[0].Trim(), CultureInfo.InvariantCulture, DateTimeStyles.None, out var f1))
                {
                    fecha = f1;
                    descripcion = cols[1].Trim();
                    decimal.TryParse(cols[2].Trim().Replace('$', ' '), NumberStyles.Any, CultureInfo.InvariantCulture, out monto);
                }
                if (monto > 0)
                {
                    lista.Add(new CrearGastoDto
                    {
                        Monto = monto,
                        Fecha = fecha == default ? DateTime.Now : fecha,
                        Descripcion = descripcion,
                        CategoriaId = 0,
                        MetodoPagoId = 0
                    });
                }
            }
            return lista;
        }
    }
}
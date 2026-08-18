using GastosPersonales.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
namespace GastosPersonales.Application.ExportStrategies
{
    public class JsonExportStrategy : IExportStrategy
    {
        public string Formato => "json";
        public string ContentType => "application/json";
        public string Extension => ".json";
        public byte[] Exportar(IEnumerable<Gasto> gastos, int mes, int anio)
        {
            var datos = gastos.Select(g => new
            {
                g.Id,
                Fecha = g.Fecha.ToString("yyyy-MM-dd"),
                g.Descripcion,
                Categoria = g.Categoria?.Nombre ?? "Sin Categoria",
                MetodoPago = g.MetodoPago?.Nombre ?? "Sin Metodo",
                g.Monto
            });
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(datos, options);
            return Encoding.UTF8.GetBytes(json);
        }
    }
}

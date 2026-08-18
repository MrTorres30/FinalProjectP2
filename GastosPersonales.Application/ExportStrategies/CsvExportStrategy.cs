using GastosPersonales.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace GastosPersonales.Application.ExportStrategies
{
    public class CsvExportStrategy : IExportStrategy
    {
        public string Formato => "csv";
        public string ContentType => "text/csv";
        public string Extension => ".csv";
        public byte[] Exportar(IEnumerable<Gasto> gastos, int mes, int anio)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Fecha,Descripcion,Categoria,MetodoPago,Monto");
            foreach (var g in gastos)
            {
                var desc = (g.Descripcion ?? "Sin descripcion").Replace(",", " ");
                var cat = (g.Categoria?.Nombre ?? "Sin categoria").Replace(",", " ");
                var mp = (g.MetodoPago?.Nombre ?? "Sin metodo").Replace(",", " ");
                sb.AppendLine($"{g.Fecha:yyyy-MM-dd},{desc},{cat},{mp},{g.Monto}");
            }
            return Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
        }
    }
}
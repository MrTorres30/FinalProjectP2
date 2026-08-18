using GastosPersonales.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace GastosPersonales.Application.ExportStrategies
{
    public class TxtExportStrategy : IExportStrategy
    {
        public string Formato => "txt";
        public string ContentType => "text/plain";
        public string Extension => ".txt";
        public byte[] Exportar(IEnumerable<Gasto> gastos, int mes, int anio)
        {
            var total = gastos.Sum(g => g.Monto);
            var sb = new StringBuilder();
            sb.AppendLine("===============================================================================");
            sb.AppendLine($"                    REPORTE DE GASTOS - MES {mes:D2}/{anio}                    ");
            sb.AppendLine("===============================================================================");
            sb.AppendLine($"Fecha Generacion : {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"Total Gastado    : {total:C}");
            sb.AppendLine("-------------------------------------------------------------------------------");
            sb.AppendLine(string.Format("{0,-12} | {1,-20} | {2,-18} | {3,-15} | {4,-10}", "Fecha", "Descripcion", "Categoria", "Metodo Pago", "Monto"));
            sb.AppendLine("-------------------------------------------------------------------------------");
            foreach (var g in gastos)
            {
                sb.AppendLine(string.Format("{0,-12:yyyy-MM-dd} | {1,-20} | {2,-18} | {3,-15} | {4,-10:C}",
                    g.Fecha,
                    (g.Descripcion?.Length > 20 ? g.Descripcion.Substring(0, 17) + "..." : g.Descripcion ?? "Sin Desc"),
                    g.Categoria?.Nombre ?? "Sin Categoria",
                    g.MetodoPago?.Nombre ?? "Sin Metodo",
                    g.Monto));
            }
            sb.AppendLine("===============================================================================");
            return Encoding.UTF8.GetBytes(sb.ToString());
        }
    }
}
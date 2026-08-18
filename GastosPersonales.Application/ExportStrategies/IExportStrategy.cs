using GastosPersonales.Domain.Entities;
using System.Collections.Generic;
namespace GastosPersonales.Application.ExportStrategies
{
    public interface IExportStrategy
    {
        string Formato { get; }     
        string ContentType { get; } 
        string Extension { get; }   
        byte[] Exportar(IEnumerable<Gasto> gastos, int mes, int anio);
    }
}
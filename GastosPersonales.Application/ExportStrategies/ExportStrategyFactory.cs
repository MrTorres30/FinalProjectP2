using System;
using System.Collections.Generic;
using System.Linq;
namespace GastosPersonales.Application.ExportStrategies
{
    public interface IExportStrategyFactory
    {
        IExportStrategy ObtenerEstrategia(string formato);
    }
    public class ExportStrategyFactory : IExportStrategyFactory
    {
        private readonly IEnumerable<IExportStrategy> _estrategias;
        public ExportStrategyFactory(IEnumerable<IExportStrategy> estrategias)
        {
            _estrategias = estrategias;
        }
        public IExportStrategy ObtenerEstrategia(string formato)
        {
            var estrategia = _estrategias.FirstOrDefault(e => e.Formato.Equals(formato, StringComparison.OrdinalIgnoreCase));

            if (estrategia == null)
            {
                throw new NotSupportedException($"El formato de exportación '{formato}' no es soportado.");
            }
            return estrategia;
        }
    }
}
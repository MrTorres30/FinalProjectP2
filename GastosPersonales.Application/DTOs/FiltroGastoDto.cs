using System;
using System.Collections.Generic;
using System.Text;

namespace GastosPersonales.Application.DTOs
{
    public class FiltroGastoDto
    {
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int? CategoriaId { get; set; }
    }
}

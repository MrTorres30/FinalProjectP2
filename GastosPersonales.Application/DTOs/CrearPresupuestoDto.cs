using System;
using System.Collections.Generic;
using System.Text;

namespace GastosPersonales.Application.DTOs
{
    public class CrearPresupuestoDto
    {
        public decimal MontoLimite { get; set; }
        public int Mes { get; set; }
        public int Anio { get; set; }
        public int CategoriaId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace GastosPersonales.Application.DTOs
{
    public class CrearGastoDto
    {
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public int CategoriaId { get; set; }
        public int MetodoPagoId { get; set; }
    }
}

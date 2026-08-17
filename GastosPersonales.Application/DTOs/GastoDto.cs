using System;
using System.Collections.Generic;
using System.Text;

namespace GastosPersonales.Application.DTOs
{
    public class GastoDto
    {
        public int Id { get; set; }
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public int CategoriaId { get; set; }
        public string NombreCategoria { get; set; } = string.Empty;
        public int MetodoPagoId { get; set; }
        public string NombreMetodoPago { get; set; } = string.Empty;
        public bool LimitePresupuestoSuperado { get; set; }
    }
}

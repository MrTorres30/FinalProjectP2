using System;
using System.Collections.Generic;
using System.Text;

namespace GastosPersonales.Domain.Entities
{
    public class Gasto
    {
        public int Id { get; set; }
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public int CategoriaId { get; set; }
        public int MetodoPagoId { get; set; }
        public int UsuarioId { get; set; }

        public virtual Usuario Usuario { get; set; } = null!;
        public virtual Categoria Categoria { get; set; } = null!;
        public virtual MetodoPago MetodoPago { get; set; } = null!;

    }
}
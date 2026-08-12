using System;
using System.Collections.Generic;
using System.Text;

namespace GastosPersonales.Domain.Entities
{
    public class MetodoPago
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Icono { get; set;}   = string.Empty;
        public bool EsActivo { get; set; }
        public int UsuarioId { get; set; }

        public virtual Usuario Usuario { get; set; } = null!;

    }
}

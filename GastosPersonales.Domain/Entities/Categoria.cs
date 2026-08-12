using System;
using System.Collections.Generic;
using System.Text;

namespace GastosPersonales.Domain.Entities
{
    public class Categoria 
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty; 
        public string Descripcion { get; set; }
        public bool EsActivo { get; set; }
        public int UsuarioId { get; set; }

        public virtual Usuario Usuario { get; set; } = null!;
    }
}

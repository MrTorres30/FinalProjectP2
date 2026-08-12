using System;
using System.Collections.Generic;
using System.Text;

namespace GastosPersonales.Domain.Entities
{
    public class Presupuesto
    {
        public int Id { get; set; }
        public decimal MontoLimite { get; set; }
        public int Mes { get; set; }
        public int Anio { get; set; }
        public int CategoriaId { get; set; }
        public int UsuarioId { get; set; }

        public virtual Usuario Usuario { get; set; } = null!;
        public virtual Categoria Categoria { get; set; } = null!;
    }


}

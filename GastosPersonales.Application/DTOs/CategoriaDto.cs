using System;
using System.Collections.Generic;
using System.Text;

namespace GastosPersonales.Application.DTOs
{
    public class CategoriaDto
    {
        public int Id { get; set; }
        public string Nombre { get; set;} = String.Empty;
        public string Descripcion { get; set; } = String.Empty;
        public bool EsActivo { get; set; }
    }
}
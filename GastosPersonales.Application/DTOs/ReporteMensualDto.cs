using System;
using System.Collections.Generic;
using System.Text;

namespace GastosPersonales.Application.DTOs
{
    public class ReporteMensualDto
    {
        public decimal TotalGastado {get; set;}
        public decimal TotalGastadoMesAnterior {get; set; }
        public decimal DiferenciaPorcentual { get; set; } 
        public List<CategoriaGastoDto> DesgloseCategorias { get; set; } = new();
        public List<CategoriaGastoDto> TopCategorias { get; set; } = new();
    }

    public class CategoriaGastoDto
    {
        public string CategoriaNombre { get; set; } = null!;
        public decimal MontoTotal { get; set; }
        public decimal Porcentaje { get; set; }
    }
}

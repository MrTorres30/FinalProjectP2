namespace GastosPersonales.Application.DTOs
{
    public class PresupuestoAlertaDto
    {
        public int CategoriaId { get; set; }
        public string CategoriaNombre { get; set; } = null!;
        public decimal MontoLimite { get; set; }
        public decimal MontoGastado { get; set; }
        public decimal PorcentajeConsumido { get; set; } 
        public string AlertaNivel { get; set; } = "Normal"; 
    }
}
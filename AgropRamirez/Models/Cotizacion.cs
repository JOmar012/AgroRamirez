using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgropRamirez.Models
{
    public class Cotizacion
    {
        public int CotizacionId { get; set; }
        public int UsuarioId { get; set; }                      // Cliente que realiza la cotización
        public DateTime FechaCotizacion { get; set; } = DateTime.Now;
        public DateTime? ValidoHasta { get; set; }
        public decimal Total { get; set; } = 0;                // Total calculado en la lógica de negocio
        public string Estado { get; set; } = "Pendiente";      // Pendiente, Aprobada, Rechazada

        // Propiedades de navegación
        [ValidateNever]
        public Usuario Usuario { get; set; } = null!;
        public ICollection<CotizacionDetalle> CotizacionDetalles { get; set; } = new List<CotizacionDetalle>();
    
    }
}

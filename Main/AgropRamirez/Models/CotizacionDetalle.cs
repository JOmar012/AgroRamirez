using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgropRamirez.Models
{
    public class CotizacionDetalle
    {
        public int CotizacionDetalleId { get; set; }
        public int CotizacionId { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; } = 1;
        public decimal PrecioUnitario { get; set; }            // Puede ser Precio o PrecioPorMayor según la cantidad
        public decimal SubTotal => Cantidad * PrecioUnitario;  // Calculado automáticamente

        // Propiedades de navegación
        [ValidateNever]
        public Cotizacion Cotizacion { get; set; } = null!;
        [ValidateNever]
        public Producto Producto { get; set; } = null!;

    }
}

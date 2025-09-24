

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AgropRamirez.Models
{
    public class CarritoDetalle
    {
        public int CarritoDetalleId { get; set; }
        public int CarritoId { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }

        // Calculada
        public decimal SubTotal => Cantidad * PrecioUnitario;

        // Propiedades de navegación
        [ValidateNever]
        public Carrito Carrito { get; set; } = null!;

        [ValidateNever]
        public Producto Producto { get; set; } = null!;
    }
}

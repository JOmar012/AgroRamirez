using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AgropRamirez.Models
{
    public class PedidoDetalle
    {
        public int PedidoDetalleId { get; set; }
        public int PedidoId { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }

        // Calculada
        public decimal SubTotal => Cantidad * PrecioUnitario;

        // Propiedades de navegación
        [ValidateNever]
        public Pedido Pedido { get; set; } = null!;

        [ValidateNever]
        public Producto Producto { get; set; } = null!;
    }
}

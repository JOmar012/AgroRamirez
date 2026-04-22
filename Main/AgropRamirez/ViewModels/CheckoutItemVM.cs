using System.Collections.Generic;
using System.Linq;

namespace AgropRamirez.ViewModels
{
    public class CheckoutItemVM
    {
        public int ProductoId { get; set; }
        public string Nombre { get; set; } = "";
        public string? Imagen { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal => Cantidad * PrecioUnitario;
        public int StockDisponible { get; set; }
    }
}

using System.Collections.Generic;
using System.Linq;

namespace AgropRamirez.ViewModels
{
    public class CheckoutVM
    {
        //Nuevo para crear desde adminin un carrtito de compras

        public int UsuarioId { get; set; }
        public string? NombreCliente { get; set; }
        public int CarritoId { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        //


        public List<CheckoutItemVM> Items { get; set; } = new();

        // 🎁 Promociones (paquetes)
        public List<CheckoutPromocionVM> Promociones { get; set; } = new();

        // 🔹 Totales combinados
        public int CantidadTotal =>
            (Items?.Sum(i => i.Cantidad) ?? 0) + (Promociones?.Sum(p => p.Cantidad) ?? 0);

        public decimal Total =>
        (Items?.Sum(i => i.Subtotal) ?? 0) + (Promociones?.Sum(p => p.Cantidad * p.PrecioTotal) ?? 0);

        // ⚠️ Validación de stock (solo afecta productos)
        public bool HayProblemasDeStock =>
            Items.Any(i => i.Cantidad > i.StockDisponible);
    }
}

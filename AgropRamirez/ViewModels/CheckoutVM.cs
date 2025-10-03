using System.Collections.Generic;
using System.Linq;

namespace AgropRamirez.ViewModels
{
    public class CheckoutVM
    {
        public List<CheckoutItemVM> Items { get; set; } = new();
        public int CantidadTotal => Items.Sum(i => i.Cantidad);
        public decimal Total => Items.Sum(i => i.Subtotal);

        // Si algún item supera el stock, bloqueamos la confirmación
        public bool HayProblemasDeStock => Items.Any(i => i.Cantidad > i.StockDisponible);
    }
}

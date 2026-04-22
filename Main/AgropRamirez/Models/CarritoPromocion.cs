using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AgropRamirez.Models
{
    public class CarritoPromocion
    {
        public int CarritoPromocionId { get; set; }

        public int CarritoId { get; set; }
        public int PromocionId { get; set; }

        public int Cantidad { get; set; } = 1;
        public decimal PrecioTotal { get; set; }

        [ValidateNever]
        public Carrito Carrito { get; set; } = null!;
        [ValidateNever]
        public Promocion Promocion { get; set; } = null!;
    }
}

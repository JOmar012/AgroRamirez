using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;

namespace AgropRamirez.Models
{
    public class Pago
    {
        public int PagoId { get; set; }
        public int PedidoId { get; set; }
        public int UsuarioId { get; set; }
        public DateTime FechaPago { get; set; }
        public decimal Monto { get; set; }
        public string MetodoPago { get; set; } = null!; // Tarjeta, Transferencia, Yape, etc.
        public string Estado { get; set; } = "Pendiente"; // Pendiente, Completado, Fallido

        // Propiedades de navegación
        [ValidateNever]
        public Pedido Pedido { get; set; } = null!;
        [ValidateNever]
        public Usuario Usuario { get; set; } = null!;
    }
}

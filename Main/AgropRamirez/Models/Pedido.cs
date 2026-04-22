using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgropRamirez.Models
{
    public class Pedido
    {
        public int PedidoId { get; set; }
        public int UsuarioId { get; set; }
        public DateTime FechaPedido { get; set; }
        public string Estado { get; set; } = "Pendiente"; // Pendiente, Confirmado, Cancelado, Enviado
        public decimal Total { get; set; }

        // Propiedades de navegación
        [ValidateNever]
        public Usuario Usuario { get; set; } = null!;
        public ICollection<PedidoDetalle> PedidoDetalles { get; set; } = new List<PedidoDetalle>();
        public ICollection<Pago> Pagos { get; set; } = new List<Pago>();
    }
}

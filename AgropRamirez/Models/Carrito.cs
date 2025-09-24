using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;


namespace AgropRamirez.Models
{
    public class Carrito
    {
        public int CarritoId { get; set; }
        public int UsuarioId { get; set; }
        public DateTime FechaCreacion { get; set; }

        // Propiedades de navegación
        [ValidateNever]
        public Usuario Usuario { get; set; } = null!;
        public ICollection<CarritoDetalle> CarritoDetalles { get; set; } = new List<CarritoDetalle>();
    }
}

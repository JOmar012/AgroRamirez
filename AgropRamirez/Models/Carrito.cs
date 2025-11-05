using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace AgropRamirez.Models
{
    public class Carrito
    {
        public int CarritoId { get; set; }
        [Display(Name = "Imagen del cliente")]
        public int UsuarioId { get; set; }

        [Display(Name = "Fecha de creación")]
        public DateTime FechaCreacion { get; set; }

        // Propiedades de navegación
        [ValidateNever]
        public Usuario Usuario { get; set; } = null!;
        public ICollection<CarritoDetalle> CarritoDetalles { get; set; } = new List<CarritoDetalle>();
        public ICollection<CarritoPromocion> CarritoPromociones { get; set; } = new List<CarritoPromocion>(); // 👈 nuevo

    }
}

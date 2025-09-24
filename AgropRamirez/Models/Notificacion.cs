using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;

namespace AgropRamirez.Models
{
    public class Notificacion
    {
        public int NotificacionId { get; set; }
        public int UsuarioId { get; set; }
        public string Titulo { get; set; } = null!;
        public string Mensaje { get; set; } = null!;
        public DateTime Fecha { get; set; }
        public bool Leido { get; set; }

        // Propiedades de navegación
        [ValidateNever]
        public Usuario Usuario { get; set; } = null!;
    }
}

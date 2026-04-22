using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgropRamirez.Models
{
    public class Usuario
    {
        public int UsuarioId { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;

        public string Dni { get; set; } = null!;          // DNI del usuario
        public string Direccion { get; set; } = null!;    // Dirección del usuario

        // Ruta de la imagen en la BD
        public string? Imagen { get; set; }
        // Propiedad auxiliar para recibir el archivo en el formulario
        [NotMapped]
        public IFormFile? ImagenFile { get; set; }

        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string Rol { get; set; } = null!; // Cliente, Administrador, Empleado
        public DateTime FechaRegistro { get; set; }

        // Propiedades de navegación
        public ICollection<Carrito> Carritos { get; set; } = new List<Carrito>();
        public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
        public ICollection<Pago> Pagos { get; set; } = new List<Pago>();
        public ICollection<Notificacion> Notificaciones { get; set; } = new List<Notificacion>();

        public ICollection<Cotizacion> Cotizaciones { get; set; } = new List<Cotizacion>();
    }
}

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace AgropRamirez.ViewModels
{
    public class UsuarioCreateVM
    {
        public int UsuarioId { get; set; }

        [Required]
        public string Nombre { get; set; } = null!;

        [Required]
        public string Apellido { get; set; } = null!;

        [Required]
        [StringLength(8, MinimumLength = 8)]
        public string Dni { get; set; } = null!;

        [Required]
        public string Direccion { get; set; } = null!;

        public string? Imagen { get; set; } // Ruta actual, útil para editar

        public IFormFile? ImagenFile { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        public string? Password { get; set; }

        [Required]
        public string Rol { get; set; } = null!; // Cliente, Admin, Empleado
    }
}

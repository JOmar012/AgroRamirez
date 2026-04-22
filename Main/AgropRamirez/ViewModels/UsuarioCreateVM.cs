using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace AgropRamirez.ViewModels
{
    public class UsuarioCreateVM
    {
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no debe exceder los 50 caracteres.")]
        [Display(Name = "Nombres")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no debe exceder los 50 caracteres.")]
        [Display(Name = "Apellidos")]
        public string Apellido { get; set; } = null!;

        [Required(ErrorMessage = "El DNI es obligatorio.")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "El DNI debe tener 8 dígitos.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "El DNI solo debe contener números.")]
        public string Dni { get; set; } = null!;

        [Required(ErrorMessage = "La dirección es obligatoria.")]
        [StringLength(150, ErrorMessage = "La dirección no debe exceder los 150 caracteres.")]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; } = null!;

        public string? Imagen { get; set; } // Ruta actual, útil para editar

        public IFormFile? ImagenFile { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido.")]
        public string Email { get; set; } = null!;

        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "El rol es obligatorio.")]
        public string Rol { get; set; } = null!; // Cliente, Admin, Empleado
    }
}

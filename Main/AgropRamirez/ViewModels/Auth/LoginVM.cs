using System.ComponentModel.DataAnnotations;

namespace AgropRamirez.ViewModels.Auth
{
    public class LoginVM
    {
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Correo no válido")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        public string? ReturnUrl { get; set; } // Para redireccionar después del login
    }
}

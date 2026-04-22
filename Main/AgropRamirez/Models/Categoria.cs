using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace AgropRamirez.Models
{
    public class Categoria
    {
        public int CategoriaId { get; set; }

        [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no debe exceder los 50 caracteres.")]
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }

        // Propiedades de navegación
        public ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}

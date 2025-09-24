using System.Collections.Generic;


namespace AgropRamirez.Models
{
    public class Categoria
    {
        public int CategoriaId { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }

        // Propiedades de navegación
        public ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}

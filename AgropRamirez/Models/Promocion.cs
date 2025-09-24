using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgropRamirez.Models
{
    public class Promocion
    {
        public int PromocionId { get; set; }
        public string Nombre { get; set; } = null!;

        // Ruta de la imagen en la BD
        public string? Imagen { get; set; }
        // Propiedad auxiliar para recibir el archivo en el formulario
        [NotMapped]
        public IFormFile? ImagenFile { get; set; }

        public string? Descripcion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public decimal Descuento { get; set; } // porcentaje
        public int? ProductoId { get; set; }

        // Propiedades de navegación
        public Producto? Producto { get; set; }
    }
}

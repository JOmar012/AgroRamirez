using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgropRamirez.Models
{
    public class Producto
    {
        public int ProductoId { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }

        public decimal? PrecioPorMayor { get; set; }           // Precio mayorista (opcional)

        public int? UmbralMayorista { get; set; }              // Desde cuántas unidades aplica (opcional)
        public int Stock { get; set; }

        // Ruta de la imagen en la BD
        public string? Imagen { get; set; }
        // Propiedad auxiliar para recibir el archivo en el formulario
        [NotMapped]
        public IFormFile? ImagenFile { get; set; }
        public int CategoriaId { get; set; }

        // Propiedades de navegación
        [ValidateNever]
        public Categoria Categoria { get; set; } = null!;
        public ICollection<Promocion> Promociones { get; set; } = new List<Promocion>();
        public ICollection<CarritoDetalle> CarritoDetalles { get; set; } = new List<CarritoDetalle>();
        public ICollection<PedidoDetalle> PedidoDetalles { get; set; } = new List<PedidoDetalle>();
        public ICollection<CotizacionDetalle> CotizacionDetalles { get; set; } = new List<CotizacionDetalle>();

    }
}

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgropRamirez.Models
{
    public class Producto
    {
        public int ProductoId { get; set; }

        [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; } = null!;

        [StringLength(300, ErrorMessage = "La descripción no puede superar los 300 caracteres.")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(0.1, 999999.99, ErrorMessage = "El precio debe estar entre 0.1 y 999999.99.")]
        public decimal Precio { get; set; }

        [Range(0.1, 999999.99, ErrorMessage = "El precio por mayor debe ser mayor que 0.")]
        public decimal? PrecioPorMayor { get; set; }           // Precio mayorista (opcional)

        [Range(1, int.MaxValue, ErrorMessage = "El umbral debe ser mayor que 0.")]
        [Display(Name = "Umbral Mayorista")]
        public int? UmbralMayorista { get; set; }              // Desde cuántas unidades aplica (opcional)

        [Required(ErrorMessage = "El stock es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
        public int Stock { get; set; }

        // Ruta de la imagen en la BD
        public string? Imagen { get; set; }
        // Propiedad auxiliar para recibir el archivo en el formulario
        [NotMapped]
        [Display(Name = "Imagen del producto")]
        public IFormFile? ImagenFile { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una categoría.")]
        [Display(Name = "Categoría")]
        public int CategoriaId { get; set; }

        // Propiedades de navegación
        [ValidateNever]
        public Categoria Categoria { get; set; } = null!;
        public ICollection<Promocion> Promociones { get; set; } = new List<Promocion>();
        public ICollection<CarritoDetalle> CarritoDetalles { get; set; } = new List<CarritoDetalle>();
        public ICollection<PedidoDetalle> PedidoDetalles { get; set; } = new List<PedidoDetalle>();
        public ICollection<CotizacionDetalle> CotizacionDetalles { get; set; } = new List<CotizacionDetalle>();

        //Descuento de promociones
        [NotMapped]
        public decimal PrecioConDescuento
        {
            get
            {
                var promoActiva = Promociones
                    ?.FirstOrDefault(p => p.FechaInicio <= DateTime.Now && p.FechaFin >= DateTime.Now);

                if (promoActiva != null)
                {
                    var descuento = Precio * (promoActiva.Descuento / 100);
                    return Math.Round(Precio - descuento, 2);
                }

                return Precio;
            }
        }
    }
}

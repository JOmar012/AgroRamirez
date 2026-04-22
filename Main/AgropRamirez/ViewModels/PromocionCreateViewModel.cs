using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgropRamirez.ViewModels
{
    public class PromocionCreateViewModel
    {
        public int PromocionId { get; set; }

        [Required(ErrorMessage = "El nombre de la promoción es obligatorio")]
        public string Nombre { get; set; } = null!;

        // Imagen
        public string? Imagen { get; set; }

        [Display(Name = "Imagen de la promoción")]
        public IFormFile? ImagenFile { get; set; }

        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "Debe ingresar una fecha de inicio")]
        [Display(Name = "Fecha de inicio")]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "Debe ingresar una fecha de fin")]
        [Display(Name = "Fecha de fin")]
        public DateTime FechaFin { get; set; }

        [Required(ErrorMessage = "Debe ingresar un descuento")]
        [Range(0, 100, ErrorMessage = "El descuento debe estar entre 0 y 100")]
        public decimal Descuento { get; set; }

        // 👇 Aquí permitimos seleccionar varios productos
        [Required(ErrorMessage = "Debe seleccionar al menos un producto")]
        [Display(Name = "Productos incluidos")]
        public List<int> ProductosSeleccionados { get; set; } = new List<int>();
    }
}

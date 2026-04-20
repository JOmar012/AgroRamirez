using AgropRamirez.Models;

namespace AgropRamirez.ViewModels
{
    public class PromocionesIndexViewModel
    {
        public IEnumerable<Promocion> Promociones { get; set; }

        public string Busqueda { get; set; }
        public int? DescuentoMin { get; set; }

        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
    }
}

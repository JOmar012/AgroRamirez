using AgropRamirez.Models;

namespace AgropRamirez.ViewModels
{
    public class CotizacionCreateViewModel
    {
        public Cotizacion Cotizacion { get; set; } = new Cotizacion();

        public List<CotizacionDetalle> Detalles { get; set; } = new List<CotizacionDetalle>();
    }
}

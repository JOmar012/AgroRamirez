using System.Collections.Generic;

namespace AgropRamirez.ViewModels
{
    /// <summary>
    /// Representa una cotización temporal generada por cualquier usuario (logueado o no).
    /// No se guarda en la base de datos.
    /// </summary>
    public class CotizacionTempDto
    {
        public int? CotizacionId { get; set; } // 👈 opcional, si existe en BD
        public string? Cliente { get; set; }
        public string Fecha { get; set; } = "";
        public string VigenteHasta { get; set; } = "";
        public string Total { get; set; } = "";
        public List<DetalleTempDto> Detalles { get; set; } = new();
    }

    /// <summary>
    /// Representa un producto dentro de la cotización temporal.
    /// </summary>
    public class DetalleTempDto
    {
        public string Producto { get; set; } = "";
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}

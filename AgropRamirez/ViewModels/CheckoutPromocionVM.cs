namespace AgropRamirez.ViewModels
{
    public class CheckoutPromocionVM
    {
        public int PromocionId { get; set; }
        public string Nombre { get; set; } = "";
        public string? Imagen { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioTotal { get; set; }

        // Lista de nombres de productos incluidos (opcional, solo para mostrar)
        public List<string> ProductosIncluidos { get; set; } = new();
    }
}

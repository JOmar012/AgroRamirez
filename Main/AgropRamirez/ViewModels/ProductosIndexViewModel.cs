using AgropRamirez.Models;

namespace AgropRamirez.ViewModels
{
    public class ProductosIndexViewModel
    {
        public IEnumerable<Producto> Productos { get; set; }

        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }

        public string Busqueda { get; set; }

        public int? CategoriaId { get; set; }
        public IEnumerable<Categoria> Categorias { get; set; }

    }
}

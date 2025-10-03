using Microsoft.EntityFrameworkCore;
using AgropRamirez.Models;

namespace AgropRamirez.Data
{
    public class AgropecuariaContext : DbContext
    {
        public AgropecuariaContext(DbContextOptions<AgropecuariaContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Promocion> Promociones { get; set; }
        public DbSet<Carrito> Carritos { get; set; }
        public DbSet<CarritoDetalle> CarritoDetalles { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<PedidoDetalle> PedidoDetalles { get; set; }
        public DbSet<Pago> Pagos { get; set; }
        public DbSet<Notificacion> Notificaciones { get; set; }
        public DbSet<Cotizacion> Cotizaciones { get; set; }
        public DbSet<CotizacionDetalle> CotizacionDetalles { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Mapear nombres exactos de las tablas
            modelBuilder.Entity<Usuario>().ToTable("Usuarios");
            modelBuilder.Entity<Categoria>().ToTable("Categorias");
            modelBuilder.Entity<Producto>().ToTable("Productos");
            modelBuilder.Entity<Promocion>().ToTable("Promociones");
            modelBuilder.Entity<Carrito>().ToTable("Carrito");
            modelBuilder.Entity<CarritoDetalle>().ToTable("CarritoDetalles");
            modelBuilder.Entity<Pedido>().ToTable("Pedidos");
            modelBuilder.Entity<PedidoDetalle>().ToTable("PedidoDetalles");
            modelBuilder.Entity<Pago>().ToTable("Pagos");
            modelBuilder.Entity<Notificacion>().ToTable("Notificaciones");
            // Tablas Cotizaciones
            modelBuilder.Entity<Cotizacion>().ToTable("Cotizaciones");

            modelBuilder.Entity<CotizacionDetalle>().ToTable("CotizacionDetalles");

            // Relaciones
            modelBuilder.Entity<Producto>()
                .HasOne(p => p.Categoria)
                .WithMany(c => c.Productos)
                .HasForeignKey(p => p.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación muchos a muchos entre Promocion y Producto
            modelBuilder.Entity<Promocion>()
                .HasMany(p => p.Productos)
                .WithMany(p => p.Promociones)
                .UsingEntity(j => j.ToTable("PromocionProductos"));

            modelBuilder.Entity<Carrito>()
                .HasOne(c => c.Usuario)
                .WithMany(u => u.Carritos)
                .HasForeignKey(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CarritoDetalle>()
                .HasOne(cd => cd.Carrito)
                .WithMany(c => c.CarritoDetalles)
                .HasForeignKey(cd => cd.CarritoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CarritoDetalle>()
                .HasOne(cd => cd.Producto)
                .WithMany(p => p.CarritoDetalles)
                .HasForeignKey(cd => cd.ProductoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pedido>()
                .HasOne(p => p.Usuario)
                .WithMany(u => u.Pedidos)
                .HasForeignKey(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PedidoDetalle>()
                .HasOne(pd => pd.Pedido)
                .WithMany(p => p.PedidoDetalles)
                .HasForeignKey(pd => pd.PedidoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PedidoDetalle>()
                .HasOne(pd => pd.Producto)
                .WithMany(p => p.PedidoDetalles)
                .HasForeignKey(pd => pd.ProductoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pago>()
                .HasOne(p => p.Pedido)
                .WithMany(pe => pe.Pagos)
                .HasForeignKey(p => p.PedidoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Pago>()
                .HasOne(p => p.Usuario)
                .WithMany(u => u.Pagos)
                .HasForeignKey(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notificacion>()
                .HasOne(n => n.Usuario)
                .WithMany(u => u.Notificaciones)
                .HasForeignKey(n => n.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

           

            // Relaciones Cotizacion
            modelBuilder.Entity<Cotizacion>()
                .HasOne(c => c.Usuario)
                .WithMany(u => u.Cotizaciones)
                .HasForeignKey(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CotizacionDetalle>()
                .HasOne(cd => cd.Cotizacion)
                .WithMany(c => c.CotizacionDetalles)
                .HasForeignKey(cd => cd.CotizacionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CotizacionDetalle>()
                .HasOne(cd => cd.Producto)
                .WithMany(p => p.CotizacionDetalles)
                .HasForeignKey(cd => cd.ProductoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

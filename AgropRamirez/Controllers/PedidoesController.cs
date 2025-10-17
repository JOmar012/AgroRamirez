using AgropRamirez.Data;
using AgropRamirez.Models;
using AgropRamirez.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AgropRamirez.Controllers
{
    public class PedidoesController : Controller
    {
        private readonly AgropecuariaContext _context;

        public PedidoesController(AgropecuariaContext context)
        {
            _context = context;
        }

        // GET: Pedidoes
        public async Task<IActionResult> Index()
        {
            var agropecuariaContext = _context.Pedidos.Include(p => p.Usuario);
            return View(await agropecuariaContext.ToListAsync());
        }

        // GET: Pedidoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedidos
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(m => m.PedidoId == id);
            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }

        // GET: Pedidoes/Create
        public IActionResult Create()
        {
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios 
                .Select(u => new
                {
                    u.UsuarioId,
                    NombreCompleto = u.Nombre + " " + u.Apellido
                }),
            "UsuarioId",
            "NombreCompleto");

            return View();
        }

        // POST: Pedidoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PedidoId,UsuarioId,FechaPedido,Estado,Total")] Pedido pedido)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pedido);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios
                .Select(u => new
                {
                    u.UsuarioId,
                    NombreCompleto = u.Nombre + " " + u.Apellido
                }),
            "UsuarioId",
            "NombreCompleto");
            return View(pedido);
        }

        // GET: Pedidoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null)
            {
                return NotFound();
            }
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios
                .Select(u => new
                {
                    u.UsuarioId,
                    NombreCompleto = u.Nombre + " " + u.Apellido
                }),
            "UsuarioId",
            "NombreCompleto");
            return View(pedido);
        }

        // POST: Pedidoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PedidoId,UsuarioId,FechaPedido,Estado,Total")] Pedido pedido)
        {
            if (id != pedido.PedidoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pedido);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PedidoExists(pedido.PedidoId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios
                .Select(u => new
                {
                    u.UsuarioId,
                    NombreCompleto = u.Nombre + " " + u.Apellido
                }),
            "UsuarioId",
            "NombreCompleto");
            return View(pedido);
        }

        // GET: Pedidoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedidos
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(m => m.PedidoId == id);
            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }

        // POST: Pedidoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido != null)
            {
                _context.Pedidos.Remove(pedido);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PedidoExists(int id)
        {
            return _context.Pedidos.Any(e => e.PedidoId == id);
        }


        //ver pedidos cliente
        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var carrito = await _context.Carritos
                .Include(c => c.CarritoDetalles).ThenInclude(cd => cd.Producto)
                .Include(c => c.CarritoPromociones).ThenInclude(cp => cp.Promocion)
                .ThenInclude(p => p.Productos)
                .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

            if (carrito == null ||
                (!carrito.CarritoDetalles.Any() && !carrito.CarritoPromociones.Any()))
            {
                TempData["Warn"] = "Tu carrito está vacío.";
                return RedirectToAction("MiCarrito", "Carritoes");
            }

            var vm = new CheckoutVM
            {
                Items = carrito.CarritoDetalles.Select(cd => new CheckoutItemVM
                {
                    ProductoId = cd.ProductoId,
                    Nombre = cd.Producto!.Nombre,
                    Imagen = cd.Producto.Imagen,
                    Cantidad = cd.Cantidad,
                    PrecioUnitario = cd.PrecioUnitario,
                    StockDisponible = cd.Producto.Stock
                }).ToList(),

                // 🔹 Agregar promociones al resumen
                Promociones = carrito.CarritoPromociones.Select(cp => new CheckoutPromocionVM
                {
                    PromocionId = cp.PromocionId,
                    Nombre = cp.Promocion!.Nombre,
                    Imagen = cp.Promocion.Imagen,
                    Cantidad = cp.Cantidad,
                    PrecioTotal = cp.PrecioTotal,
                    ProductosIncluidos = cp.Promocion.Productos.Select(p => p.Nombre).ToList()
                }).ToList()
            };

            

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarPedido()
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var carrito = await _context.Carritos
                .Include(c => c.CarritoDetalles)
                    .ThenInclude(cd => cd.Producto)
                .Include(c => c.CarritoPromociones)
                    .ThenInclude(cp => cp.Promocion)
                .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

            // ⚠️ Validar si el carrito está vacío
            if (carrito == null ||
                (!carrito.CarritoDetalles.Any() && !carrito.CarritoPromociones.Any()))
            {
                TempData["Warn"] = "Tu carrito está vacío.";
                return RedirectToAction("MiCarrito", "Carritoes");
            }

            // ⚠️ Validar stock de productos
            foreach (var item in carrito.CarritoDetalles)
            {
                if (item.Cantidad > item.Producto.Stock)
                {
                    TempData["Warn"] = $"El producto {item.Producto.Nombre} no tiene suficiente stock.";
                    return RedirectToAction("Checkout");
                }
            }

            // 💰 Calcular total incluyendo promociones
            var totalPedido =
                carrito.CarritoDetalles.Sum(cd => cd.Cantidad * cd.PrecioUnitario)
                + carrito.CarritoPromociones.Sum(cp => cp.Cantidad * cp.PrecioTotal);

            // 🧾 Crear pedido (solo con productos)
            var pedido = new Pedido
            {
                UsuarioId = usuarioId,
                FechaPedido = DateTime.Now,
                Estado = "Pendiente de pago",
                Total = totalPedido,
                PedidoDetalles = carrito.CarritoDetalles.Select(cd => new PedidoDetalle
                {
                    ProductoId = cd.ProductoId,
                    Cantidad = cd.Cantidad,
                    PrecioUnitario = cd.PrecioUnitario
                }).ToList()
            };

            // ✅ Guardar el pedido
            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            // 🚀 Redirigir al pago
            return RedirectToAction("CreatePagoCliente", "Pagoes", new { pedidoId = pedido.PedidoId });
        }



    }
}

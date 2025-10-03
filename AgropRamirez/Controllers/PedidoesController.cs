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
                .Include(c => c.CarritoDetalles)
                    .ThenInclude(cd => cd.Producto)
                .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

            if (carrito == null || !carrito.CarritoDetalles.Any())
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
                    Imagen = cd.Producto.Imagen, // ajusta si usas ImagenRuta
                    Cantidad = cd.Cantidad,
                    PrecioUnitario = cd.PrecioUnitario,
                    StockDisponible = cd.Producto.Stock
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
                .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

            if (carrito == null || !carrito.CarritoDetalles.Any())
            {
                TempData["Warn"] = "Tu carrito está vacío.";
                return RedirectToAction("MiCarrito", "Carritoes");
            }

            // ⚠️ Revalidar stock por seguridad
            foreach (var item in carrito.CarritoDetalles)
            {
                if (item.Cantidad > item.Producto.Stock)
                {
                    TempData["Warn"] = $"El producto {item.Producto.Nombre} no tiene suficiente stock. Disponible: {item.Producto.Stock}";
                    return RedirectToAction("Checkout");
                }
            }

            // Calcular total del pedido
            var totalPedido = carrito.CarritoDetalles.Sum(cd => cd.Cantidad * cd.PrecioUnitario);

            // Crear Pedido
            var pedido = new Pedido
            {
                UsuarioId = usuarioId,
                FechaPedido = DateTime.Now,
                Estado = "Pendiente",   // 👈 estado inicial
                Total = totalPedido,    // 👈 total general del pedido
                PedidoDetalles = new List<PedidoDetalle>()
            };

            foreach (var item in carrito.CarritoDetalles)
            {
                // Descontar stock
                item.Producto.Stock -= item.Cantidad;

                pedido.PedidoDetalles.Add(new PedidoDetalle
                {
                    ProductoId = item.ProductoId,
                    Cantidad = item.Cantidad,
                    PrecioUnitario = item.PrecioUnitario
                });
            }

            _context.Pedidos.Add(pedido);

            // Vaciar carrito
            _context.CarritoDetalles.RemoveRange(carrito.CarritoDetalles);

            await _context.SaveChangesAsync();

            // Redirigir al registro de pago
            return RedirectToAction("Registrar", "Pagos", new { pedidoId = pedido.PedidoId });
        }

    }
}

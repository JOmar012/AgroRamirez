using AgropRamirez.Data;
using AgropRamirez.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Security.Claims;


namespace AgropRamirez.Controllers
{
    [Authorize] // Solo usuarios logueados
    public class CarritoesController : Controller
    {
        private readonly AgropecuariaContext _context;

        public CarritoesController(AgropecuariaContext context)
        {
            _context = context;
        }

        // GET: Carritoes
        public async Task<IActionResult> Index()
        {
            var agropecuariaContext = _context.Carritos.Include(c => c.Usuario);
            return View(await agropecuariaContext.ToListAsync());
        }

        // GET: Carritoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carrito = await _context.Carritos
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(m => m.CarritoId == id);
            if (carrito == null)
            {
                return NotFound();
            }

            return View(carrito);
        }

        // GET: Carritoes/Create
        public IActionResult Create()
        {
            ViewData["UsuarioId"] = new SelectList(
                _context.Usuarios
                    .Select(u => new
                    {
                        u.UsuarioId,
                        NombreCompleto = u.Nombre + " " + u.Apellido
                    }),
            "UsuarioId",
            "NombreCompleto");

            return View();
        }

        // POST: Carritoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CarritoId,UsuarioId,FechaCreacion")] Carrito carrito)
        {
            if (ModelState.IsValid)
            {
                _context.Add(carrito);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["UsuarioId"] = new SelectList(
                _context.Usuarios
                    .Select(u => new
                    {
                        u.UsuarioId,
                        NombreCompleto = u.Nombre + " " + u.Apellido
                    }),
            "UsuarioId",
            "NombreCompleto");

            return View(carrito);
        }

        // GET: Carritoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carrito = await _context.Carritos.FindAsync(id);
            if (carrito == null)
            {
                return NotFound();
            }

            ViewData["UsuarioId"] = new SelectList(
                _context.Usuarios
                    .Select(u => new
                    {
                        u.UsuarioId,
                        NombreCompleto = u.Nombre + " " + u.Apellido
                    }),
            "UsuarioId",
            "NombreCompleto");

            return View(carrito);
        }

        // POST: Carritoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CarritoId,UsuarioId,FechaCreacion")] Carrito carrito)
        {
            if (id != carrito.CarritoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(carrito);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarritoExists(carrito.CarritoId))
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
            ViewData["UsuarioId"] = new SelectList(
                 _context.Usuarios
                     .Select(u => new
                     {
                         u.UsuarioId,
                         NombreCompleto = u.Nombre + " " + u.Apellido
                     }),
             "UsuarioId",
             "NombreCompleto");
            return View(carrito);
        }

        // GET: Carritoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carrito = await _context.Carritos
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(m => m.CarritoId == id);
            if (carrito == null)
            {
                return NotFound();
            }

            return View(carrito);
        }

        // POST: Carritoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var carrito = await _context.Carritos.FindAsync(id);
            if (carrito != null)
            {
                _context.Carritos.Remove(carrito);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarritoExists(int id)
        {
            return _context.Carritos.Any(e => e.CarritoId == id);
        }


        //Método para añadir al carrito

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Agregar(int productoId, int cantidad = 1)
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var carrito = await _context.Carritos
                .Include(c => c.CarritoDetalles)
                .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

            if (carrito == null)
            {
                carrito = new Carrito
                {
                    UsuarioId = usuarioId,
                    FechaCreacion = DateTime.Now
                };
                _context.Carritos.Add(carrito);
                await _context.SaveChangesAsync();
            }

            var producto = await _context.Productos.FindAsync(productoId);
            if (producto == null) return NotFound();

            var detalle = carrito.CarritoDetalles.FirstOrDefault(cd => cd.ProductoId == productoId);

            if (detalle != null)
            {
                if (detalle.Cantidad + cantidad > producto.Stock)
                {
                    return Json(new
                    {
                        success = false,
                        warning = true,
                        mensaje = $"No hay suficiente stock de {producto.Nombre}. Stock disponible: {producto.Stock}"
                    });
                }

                detalle.Cantidad += cantidad;
            }
            else
            {
                if (cantidad > producto.Stock)
                {
                    return Json(new
                    {
                        success = false,
                        warning = true,
                        mensaje = $"Stock insuficiente. Solo hay {producto.Stock} unidades disponibles."
                    });
                }

                carrito.CarritoDetalles.Add(new CarritoDetalle
                {
                    ProductoId = producto.ProductoId,
                    Cantidad = cantidad,
                    PrecioUnitario = producto.Precio
                });
            }

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                mensaje = $"{producto.Nombre} añadido al carrito 🛒"
            });
        }

        //Cantidad al carrito

        [HttpGet]
        public async Task<IActionResult> ObtenerCantidad()
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var cantidad = await _context.CarritoDetalles
                .Where(cd => cd.Carrito.UsuarioId == usuarioId)
                .SumAsync(cd => cd.Cantidad);

            return Json(cantidad);
        }

        public async Task<IActionResult> MiCarrito()
        {
            // Obtén el usuario actual
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            // Busca el carrito del usuario con los detalles
            var carrito = await _context.Carritos
                .Include(c => c.CarritoDetalles)
                .ThenInclude(cd => cd.Producto)
                .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

            if (carrito == null)
            {
                // si aún no tiene carrito, crea uno vacío
                carrito = new Carrito
                {
                    UsuarioId = usuarioId,
                    FechaCreacion = DateTime.Now,
                    CarritoDetalles = new List<CarritoDetalle>()
                };
                _context.Carritos.Add(carrito);
                await _context.SaveChangesAsync();
            }

            return View(carrito);
        }

        //Actualizar cantidad al carrito
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarCantidad(int detalleId, int cantidad)
        {
            var detalle = await _context.CarritoDetalles
        .Include(cd => cd.Carrito)
        .Include(cd => cd.Producto)
        .FirstOrDefaultAsync(cd => cd.CarritoDetalleId == detalleId);

            if (detalle == null)
                return Json(new { success = false, message = "Detalle no encontrado" });

            // ✅ Validar stock
            if (cantidad > detalle.Producto.Stock)
            {
                return Json(new
                {
                    success = false,
                    warning = true,
                    message = $"Solo hay {detalle.Producto.Stock} unidades de {detalle.Producto.Nombre}."
                });
            }

            if (cantidad <= 0)
            {
                var usuarioId = detalle.Carrito.UsuarioId;

                _context.CarritoDetalles.Remove(detalle);
                await _context.SaveChangesAsync();

                var totalDespues = await _context.CarritoDetalles
                    .Where(cd => cd.Carrito.UsuarioId == usuarioId)
                    .SumAsync(cd => (decimal?)(cd.Cantidad * cd.PrecioUnitario)) ?? 0;

                return Json(new
                {
                    success = true,
                    eliminado = true,
                    subtotal = 0m,
                    total = totalDespues
                });
            }
            else
            {
                detalle.Cantidad = cantidad;
                await _context.SaveChangesAsync();

                var subtotalActual = detalle.Cantidad * detalle.PrecioUnitario;

                var totalDespues = await _context.CarritoDetalles
                    .Where(cd => cd.Carrito.UsuarioId == detalle.Carrito.UsuarioId)
                    .SumAsync(cd => (decimal?)(cd.Cantidad * cd.PrecioUnitario)) ?? 0;

                return Json(new
                {
                    success = true,
                    eliminado = false,
                    subtotal = subtotalActual,
                    total = totalDespues
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarDetalle(int detalleId)
        {
            var detalle = await _context.CarritoDetalles
                 .Include(cd => cd.Carrito)
                 .FirstOrDefaultAsync(cd => cd.CarritoDetalleId == detalleId);

            if (detalle == null)
                return Json(new { success = false, message = "Producto no encontrado" });

            var usuarioId = detalle.Carrito.UsuarioId;

            _context.CarritoDetalles.Remove(detalle);
            await _context.SaveChangesAsync();

            // Calcular el nuevo total sin usar SubTotal
            var totalActualizado = await _context.CarritoDetalles
                .Where(cd => cd.Carrito.UsuarioId == usuarioId)
                .SumAsync(cd => (decimal?)(cd.Cantidad * cd.PrecioUnitario)) ?? 0;

            return Json(new
            {
                success = true,
                total = totalActualizado
            });
        }
    }
}

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

            // 🛒 Buscar carrito del usuario
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

            // 🔎 Buscar producto
            var producto = await _context.Productos.FirstOrDefaultAsync(p => p.ProductoId == productoId);
            if (producto == null)
                return NotFound();

            // ⚙️ Calcular precio base
            var precioFinal = producto.Precio;

            // 🟢 Verificar si aplica precio mayorista
            if (producto.UmbralMayorista.HasValue && producto.PrecioPorMayor.HasValue)
            {
                // Cantidad total en carrito (actual + nueva)
                var detalleExistente = carrito.CarritoDetalles.FirstOrDefault(cd => cd.ProductoId == productoId);
                int cantidadTotal = detalleExistente != null ? detalleExistente.Cantidad + cantidad : cantidad;

                if (cantidadTotal >= producto.UmbralMayorista.Value)
                {
                    precioFinal = producto.PrecioPorMayor.Value;
                }
            }

            // ⚠️ Verificar si ya existe en el carrito
            var detalle = carrito.CarritoDetalles.FirstOrDefault(cd => cd.ProductoId == productoId);

            if (detalle != null)
            {
                // 🧮 Validar stock antes de sumar
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
                detalle.PrecioUnitario = precioFinal; // 🔄 actualiza el precio (normal o mayorista)
            }
            else
            {
                // 🧮 Validar stock inicial
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
                    PrecioUnitario = precioFinal // ✅ Aplica precio normal o mayorista
                });
            }

            await _context.SaveChangesAsync();

            // 🔔 Mensaje adaptado
            string mensajeFinal = (producto.UmbralMayorista.HasValue && cantidad >= producto.UmbralMayorista.Value)
                ? $"{producto.Nombre} añadido al carrito con precio mayorista 🛒"
                : $"{producto.Nombre} añadido al carrito 🛒";

            return Json(new
            {
                success = true,
                mensaje = mensajeFinal
            });
        }

        //Cantidad al carrito

        [HttpGet]
        public async Task<IActionResult> ObtenerCantidad()
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            // Sumar productos
            var cantidadProductos = await _context.CarritoDetalles
                .Where(cd => cd.Carrito.UsuarioId == usuarioId)
                .SumAsync(cd => (int?)cd.Cantidad) ?? 0;

            // Sumar promociones
            var cantidadPromociones = await _context.CarritoPromociones
                .Where(cp => cp.Carrito.UsuarioId == usuarioId)
                .SumAsync(cp => (int?)cp.Cantidad) ?? 0;

            var cantidadTotal = cantidadProductos + cantidadPromociones;

            return Json(cantidadTotal);

        }

        public async Task<IActionResult> MiCarrito()
        {
            // Obtén el usuario actual
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            // Busca el carrito del usuario con los detalles
            var carrito = await _context.Carritos
                .Include(c => c.CarritoDetalles)
                    .ThenInclude(cd => cd.Producto)
                .Include(c => c.CarritoPromociones)
                    .ThenInclude(cp => cp.Promocion)
                        .ThenInclude(p => p.Productos)
                .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

            if (carrito == null)
            {
                // si aún no tiene carrito, crea uno vacío
                carrito = new Carrito
                {
                    UsuarioId = usuarioId,
                    FechaCreacion = DateTime.Now,
                    CarritoDetalles = new List<CarritoDetalle>(),
                    CarritoPromociones = new List<CarritoPromocion>()
                };
                _context.Carritos.Add(carrito);
                await _context.SaveChangesAsync();
            }

            return View(carrito);
        }

        // Actualizar cantidad al carrito
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

            var usuarioId = detalle.Carrito.UsuarioId;

            if (cantidad <= 0)
            {
                // 🔹 Eliminar producto si la cantidad llega a 0
                _context.CarritoDetalles.Remove(detalle);
                await _context.SaveChangesAsync();
            }
            else
            {
                // 🔹 Actualizar cantidad
                detalle.Cantidad = cantidad;
                await _context.SaveChangesAsync();

                // 🧮 Verificar si aplica precio mayorista
                var producto = detalle.Producto;
                decimal precioFinal = producto.Precio;

                if (producto.UmbralMayorista.HasValue && producto.PrecioPorMayor.HasValue &&
                        cantidad >= producto.UmbralMayorista.Value)
                {
                    precioFinal = producto.PrecioPorMayor.Value;
                }

                detalle.PrecioUnitario = precioFinal;

                await _context.SaveChangesAsync();
            }

            // 🔹 Calcular total actualizado (productos + promociones)
            var totalProductos = await _context.CarritoDetalles
                .Where(cd => cd.Carrito.UsuarioId == usuarioId)
                .SumAsync(cd => (decimal?)(cd.Cantidad * cd.PrecioUnitario)) ?? 0;

            var totalPromociones = await _context.CarritoPromociones
                .Where(cp => cp.Carrito.UsuarioId == usuarioId)
                .SumAsync(cp => (decimal?)(cp.Cantidad * cp.PrecioTotal)) ?? 0;

            var totalDespues = totalProductos + totalPromociones;

            // 🔹 Subtotal del producto actualizado
            var subtotalActual = cantidad > 0 ? detalle.Cantidad * detalle.PrecioUnitario : 0m;


            // 🔔 Mensaje informativo (opcional)
            string mensaje = "";
            if (cantidad > 0)
            {
                if (detalle.Producto.UmbralMayorista.HasValue && cantidad >= detalle.Producto.UmbralMayorista.Value)
                    mensaje = $"Se aplicó el precio mayorista a {detalle.Producto.Nombre}.";
                else
                    mensaje = $"Precio normal aplicado a {detalle.Producto.Nombre}.";
            }

            return Json(new
            {
                success = true,
                eliminado = cantidad <= 0,
                subtotal = subtotalActual,
                total = totalDespues,
                mensaje
            });
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

            // 🔹 Eliminar producto del carrito
            _context.CarritoDetalles.Remove(detalle);
            await _context.SaveChangesAsync();

            // 🔹 Calcular total global (productos + promociones)
            var totalProductos = await _context.CarritoDetalles
                .Where(cd => cd.Carrito.UsuarioId == usuarioId)
                .SumAsync(cd => (decimal?)(cd.Cantidad * cd.PrecioUnitario)) ?? 0;

            var totalPromociones = await _context.CarritoPromociones
                .Where(cp => cp.Carrito.UsuarioId == usuarioId)
                .SumAsync(cp => (decimal?)(cp.Cantidad * cp.PrecioTotal)) ?? 0;

            var totalActualizado = totalProductos + totalPromociones;

            return Json(new
            {
                success = true,
                total = totalActualizado
            });
        }

        // ✅ Agregar promoción al carrito
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarPromocionCompacta(int promocionId)
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var carrito = await _context.Carritos
                .Include(c => c.CarritoPromociones)
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

            var promo = await _context.Promociones
                .Include(p => p.Productos)
                .FirstOrDefaultAsync(p => p.PromocionId == promocionId);

            if (promo == null || !promo.Productos.Any())
                return Json(new { success = false, mensaje = "Promoción no encontrada o sin productos." });

            // ✅ Verificar stock de cada producto de la promoción
            foreach (var prod in promo.Productos)
            {
                if (prod.Stock <= 0)
                {
                    return Json(new
                    {
                        success = false,
                        warning = true,
                        mensaje = $"El producto '{prod.Nombre}' de esta promoción no tiene stock disponible."
                    });
                }
            }

            // ✅ Calcular total con descuento
            var totalPromo = promo.Productos.Sum(p => p.Precio) * (1 - (promo.Descuento / 100));

            // ✅ Verificar si ya existe en el carrito
            var existente = carrito.CarritoPromociones.FirstOrDefault(cp => cp.PromocionId == promocionId);

            // 🔸 Calcular cantidad total actual (existente + nueva)
            int cantidadActual = existente?.Cantidad ?? 0;
            int nuevaCantidad = cantidadActual + 1;

            // 🚫 Validar límite máximo de 3 promociones iguales por usuario
            if (nuevaCantidad > 3)
            {
                return Json(new
                {
                    success = false,
                    warning = true,
                    mensaje = $"⚠️ Solo puedes agregar la promoción '{promo.Nombre}' hasta 3 veces por usuario."
                });
            }

            // ✅ Si no supera el límite, agregar o incrementar
            if (existente != null)
            {
                existente.Cantidad++;
                existente.PrecioTotal = Math.Round((decimal)totalPromo * existente.Cantidad, 2);
            }
            else
            {
                carrito.CarritoPromociones.Add(new CarritoPromocion
                {
                    PromocionId = promo.PromocionId,
                    Cantidad = 1,
                    PrecioTotal = Math.Round((decimal)totalPromo, 2)
                });
            }

            await _context.SaveChangesAsync();

            // ✅ Respuesta al cliente (AJAX)
            return Json(new
            {
                success = true,
                mensaje = $"La promoción '{promo.Nombre}' fue añadida al carrito 🛒",
                total = Math.Round((decimal)totalPromo, 2)
            });
        }

        //Eliminar promoción del carrito
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarPromocion(int carritoPromocionId)
        {
            var promo = await _context.CarritoPromociones
                .Include(cp => cp.Carrito)
                .FirstOrDefaultAsync(cp => cp.CarritoPromocionId == carritoPromocionId);

            if (promo == null)
                return Json(new { success = false, message = "Promoción no encontrada" });

            var usuarioId = promo.Carrito.UsuarioId;

            // 🔹 Eliminar la promoción del carrito
            _context.CarritoPromociones.Remove(promo);
            await _context.SaveChangesAsync();

            // 🔹 Calcular el total general (productos + promociones)
            var totalActualizado = await CalcularTotalGeneralAsync(usuarioId);

            return Json(new
            {
                success = true,
                total = totalActualizado
            });
        }
        // 🔹 Cálculo total del carrito (productos + promociones)
        private async Task<decimal> CalcularTotalGeneralAsync(int usuarioId)
        {
            var totalProductos = await _context.CarritoDetalles
                .Where(cd => cd.Carrito.UsuarioId == usuarioId)
                .SumAsync(cd => (decimal?)(cd.Cantidad * cd.PrecioUnitario)) ?? 0;

            var totalPromociones = await _context.CarritoPromociones
                .Where(cp => cp.Carrito.UsuarioId == usuarioId)
                .SumAsync(cp => (decimal?)(cp.Cantidad * cp.PrecioTotal)) ?? 0;

            return totalProductos + totalPromociones;
        }

    }
}

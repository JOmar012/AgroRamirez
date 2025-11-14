using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AgropRamirez.Data;
using AgropRamirez.Models;

namespace AgropRamirez.Controllers
{
    public class PagoesController : Controller
    {
        private readonly AgropecuariaContext _context;

        public PagoesController(AgropecuariaContext context)
        {
            _context = context;
        }

        // GET: Pagoes
        public async Task<IActionResult> Index()
        {
            var agropecuariaContext = _context.Pagos.Include(p => p.Pedido).Include(p => p.Usuario);
            return View(await agropecuariaContext.ToListAsync());
        }

        // GET: Pagoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pago = await _context.Pagos
                .Include(p => p.Pedido)
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(m => m.PagoId == id);
            if (pago == null)
            {
                return NotFound();
            }

            return View(pago);
        }

            // GET: Pagoes/Create

            [HttpGet]
            public async Task<IActionResult> Create(int pedidoId)
            {

                // 🔥 Limpiamos cualquier mensaje previo
                //TempData.Remove("Success");

                // 🔍 Buscar el pedido con cliente
                var pedido = await _context.Pedidos
                    .Include(p => p.Usuario)
                    .FirstOrDefaultAsync(p => p.PedidoId == pedidoId);

                if (pedido == null)
                    return NotFound();

                // 💰 Crear el modelo con datos por defecto
                var model = new Pago
                {
                    PedidoId = pedido.PedidoId,
                    UsuarioId = pedido.UsuarioId,
                    FechaPago = DateTime.Now,
                    Monto = pedido.Total,
                    MetodoPago = "Efectivo", // puedes ajustar por defecto
                    Estado = "Pagado"
                };

                // Enviar nombre del cliente a la vista mediante ViewBag
                ViewBag.NombreCliente = $"{pedido.Usuario.Nombre} {pedido.Usuario.Apellido}";
                ViewBag.Total = pedido.Total.ToString("F2");

                return View("Create", model);
            }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Pago pago)
        {
            if (!ModelState.IsValid)
            {
                var pedidoError = await _context.Pedidos
                    .Include(p => p.Usuario)
                    .FirstOrDefaultAsync(p => p.PedidoId == pago.PedidoId);

                ViewBag.NombreCliente = $"{pedidoError.Usuario.Nombre} {pedidoError.Usuario.Apellido}";
                ViewBag.Total = pedidoError.Total.ToString("F2");
                return View("Create", pago);
            }

            // ⚠️ 1. Verificar si el pedido ya tiene un pago registrado
            bool yaPagado = await _context.Pagos.AnyAsync(p => p.PedidoId == pago.PedidoId);

            if (yaPagado)
            {
                // Si ya existe un pago, mostrar alerta de advertencia
                TempData["PagoDuplicado"] = true;
                return RedirectToAction("Create", new { pedidoId = pago.PedidoId });
            }

            // 💰 2. Registrar el nuevo pago
            pago.Estado = "Pagado";
            _context.Pagos.Add(pago);
            await _context.SaveChangesAsync();

            // 📦 3. Buscar pedido con detalles y descontar stock
            var pedido = await _context.Pedidos
                .Include(p => p.PedidoDetalles)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(p => p.PedidoId == pago.PedidoId);

            if (pedido != null)
            {
                foreach (var detalle in pedido.PedidoDetalles)
                {
                    if (detalle.Producto != null)
                    {
                        detalle.Producto.Stock -= detalle.Cantidad;
                        if (detalle.Producto.Stock < 0)
                            detalle.Producto.Stock = 0;

                        _context.Update(detalle.Producto);
                    }
                }

                pedido.Estado = "Pagado";
                _context.Update(pedido);
                await _context.SaveChangesAsync();
            }

            // 🚀 4. Bandera para alerta de éxito
            TempData["PagoExitoso"] = true;

            // Redirigir a la misma vista para mostrar alerta y luego ir al catálogo
            return RedirectToAction("Create", new { pedidoId = pago.PedidoId });
        }

        //public IActionResult Create()
        //{
        //    ViewData["PedidoId"] = new SelectList(
        //        _context.Pedidos.Select(p => new
        //        {
        //            p.PedidoId,
        //            Fecha = p.FechaPedido.ToString("dd/MM/yyyy")
        //        }),
        //        "PedidoId",
        //        "Fecha"
        //    );

        //    ViewData["UsuarioId"] = new SelectList(
        //        _context.Usuarios.Select(u => new
        //        {
        //            u.UsuarioId,
        //            NombreCompleto = u.Nombre + " " + u.Apellido
        //        }),
        //        "UsuarioId",
        //        "NombreCompleto"
        //    );

        //    return View();
        //}

        //// POST: Pagoes/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("PagoId,PedidoId,UsuarioId,FechaPago,Monto,MetodoPago,Estado")] Pago pago)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(pago);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }

        //    ViewData["PedidoId"] = new SelectList(
        //        _context.Pedidos.Select(p => new
        //        {
        //            p.PedidoId,
        //            Fecha = p.FechaPedido.ToString("dd/MM/yyyy")
        //        }),
        //        "PedidoId",
        //        "Fecha",
        //        pago.PedidoId
        //    );

        //    ViewData["UsuarioId"] = new SelectList(
        //        _context.Usuarios.Select(u => new
        //        {
        //            u.UsuarioId,
        //            NombreCompleto = u.Nombre + " " + u.Apellido
        //        }),
        //        "UsuarioId",
        //        "NombreCompleto",
        //        pago.UsuarioId
        //    );
        //    return View(pago);
        //}

        // GET: Pagoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pago = await _context.Pagos.FindAsync(id);
            if (pago == null)
            {
                return NotFound();
            }
            ViewData["PedidoId"] = new SelectList(
            _context.Pedidos.Select(p => new
            {
                p.PedidoId,
                Fecha = p.FechaPedido.ToString("dd/MM/yyyy")
            }),
            "PedidoId",
            "Fecha",
            pago.PedidoId
    );

            ViewData["UsuarioId"] = new SelectList(
                _context.Usuarios.Select(u => new
                {
                    u.UsuarioId,
                    NombreCompleto = u.Nombre + " " + u.Apellido
                }),
                "UsuarioId",
                "NombreCompleto",
                pago.UsuarioId
            );
            return View(pago);
        }

        // POST: Pagoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PagoId,PedidoId,UsuarioId,FechaPago,Monto,MetodoPago,Estado")] Pago pago)
        {
            if (id != pago.PagoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pago);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PagoExists(pago.PagoId))
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

            ViewData["PedidoId"] = new SelectList(
            _context.Pedidos.Select(p => new
            {
                p.PedidoId,
                Fecha = p.FechaPedido.ToString("dd/MM/yyyy")
            }),
            "PedidoId",
            "Fecha",
            pago.PedidoId
    );

            ViewData["UsuarioId"] = new SelectList(
                _context.Usuarios.Select(u => new
                {
                    u.UsuarioId,
                    NombreCompleto = u.Nombre + " " + u.Apellido
                }),
                "UsuarioId",
                "NombreCompleto",
                pago.UsuarioId
            );
            return View(pago);
        }

        // GET: Pagoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pago = await _context.Pagos
                .Include(p => p.Pedido)
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(m => m.PagoId == id);
            if (pago == null)
            {
                return NotFound();
            }

            return View(pago);
        }

        // POST: Pagoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pago = await _context.Pagos.FindAsync(id);
            if (pago != null)
            {
                _context.Pagos.Remove(pago);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PagoExists(int id)
        {
            return _context.Pagos.Any(e => e.PagoId == id);
        }

        //Pagos create cliente

        // GET: Pagoes/CreatePagoCliente
        public async Task<IActionResult> CreatePagoCliente(int pedidoId)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Usuario)
                .Include(p => p.PedidoDetalles)
                    .ThenInclude(pd => pd.Producto)
                .FirstOrDefaultAsync(p => p.PedidoId == pedidoId);

            if (pedido == null)
                return NotFound();

            // 🧮 Calcular monto total del pedido (productos + promociones)
            // Las promociones ya están sumadas en el campo Total del pedido
            var montoTotal = pedido.Total;

            var pago = new Pago
            {
                PedidoId = pedido.PedidoId,
                UsuarioId = pedido.UsuarioId,
                FechaPago = DateTime.Now,
                Monto = montoTotal,
                Estado = "Pendiente"
            };

            return View(pago);
        }

        // POST: Pagoes/CreatePagoCliente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePagoCliente([Bind("PedidoId,UsuarioId,FechaPago,Monto,MetodoPago,Estado")] Pago pago)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, mensaje = "Datos inválidos." });

            try
            {
                // ⚠️ 1️⃣ Verificar si ya existe un pago para este pedido
                bool pagoExistente = await _context.Pagos.AnyAsync(p => p.PedidoId == pago.PedidoId);
                if (pagoExistente)
                {
                    return Json(new
                    {
                        success = false,
                        warning = true,
                        mensaje = "⚠️ Este pedido ya cuenta con un pago registrado. No es posible registrar otro."
                    });
                }

                // ✅ Buscar el pedido y verificar que exista
                var pedido = await _context.Pedidos
                    .Include(p => p.PedidoDetalles)
                        .ThenInclude(pd => pd.Producto)
                    .FirstOrDefaultAsync(p => p.PedidoId == pago.PedidoId);

                if (pedido == null)
                    return Json(new { success = false, mensaje = "No se encontró el pedido asociado." });

                // ✅ Verificar stock de cada producto del pedido antes de pagar
                foreach (var det in pedido.PedidoDetalles)
                {
                    if (det.Producto == null)
                        continue;

                    if (det.Producto.Stock < det.Cantidad)
                    {
                        return Json(new
                        {
                            success = false,
                            warning = true,
                            mensaje = $"El producto '{det.Producto.Nombre}' no tiene stock suficiente. Disponible: {det.Producto.Stock}, solicitado: {det.Cantidad}."
                        });
                    }
                }

                // ✅ Verificar stock de productos incluidos en promociones
                var carritoPromos = await _context.CarritoPromociones
                    .Include(cp => cp.Promocion)
                        .ThenInclude(p => p.Productos)
                    .Where(cp => cp.Carrito.UsuarioId == pago.UsuarioId)
                    .ToListAsync();

                foreach (var cp in carritoPromos)
                {
                    foreach (var prod in cp.Promocion.Productos)
                    {
                        if (prod.Stock < cp.Cantidad)
                        {
                            return Json(new
                            {
                                success = false,
                                warning = true,
                                mensaje = $"El producto '{prod.Nombre}' de la promoción '{cp.Promocion.Nombre}' no tiene stock suficiente."
                            });
                        }
                    }
                }

                // ✅ Si todo tiene stock, registrar el pago
                pago.Estado = "Pagado";
                _context.Pagos.Add(pago);
                await _context.SaveChangesAsync();

                // 🔔 Crear notificación al administrador
                await NotificarAdministradorAsync(pago);

                // ✅ Actualizar estado del pedido
                pedido.Estado = "Pagado";

                // ✅ Descontar stock de productos individuales
                foreach (var det in pedido.PedidoDetalles)
                {
                    if (det.Producto != null)
                        det.Producto.Stock -= det.Cantidad;
                }

                // ✅ Descontar stock de productos incluidos en promociones
                foreach (var cp in carritoPromos)
                {
                    foreach (var prod in cp.Promocion.Productos)
                    {
                        prod.Stock -= cp.Cantidad;
                    }
                }

                // ✅ Vaciar carrito del usuario (productos + promociones)
                var carrito = await _context.Carritos
                    .Include(c => c.CarritoDetalles)
                    .Include(c => c.CarritoPromociones)
                    .FirstOrDefaultAsync(c => c.UsuarioId == pago.UsuarioId);

                if (carrito != null)
                {
                    if (carrito.CarritoDetalles.Any())
                        _context.CarritoDetalles.RemoveRange(carrito.CarritoDetalles);

                    if (carrito.CarritoPromociones.Any())
                        _context.CarritoPromociones.RemoveRange(carrito.CarritoPromociones);
                }

                await _context.SaveChangesAsync();

                // ✅ Respuesta AJAX
                return Json(new
                {
                    success = true,
                    mensaje = $"Tu pago de S/ {pago.Monto:0.00} con {pago.MetodoPago} fue registrado correctamente. ¡Gracias por tu compra! 🎉",
                    total = pago.Monto,
                    metodo = pago.MetodoPago
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = "Error al registrar el pago: " + ex.Message });
            }
        }

        private async Task NotificarAdministradorAsync(Pago pago)
        {
            // 🔍 Buscar administrador (por rol o condición)
            var admin = await _context.Usuarios.FirstOrDefaultAsync(u => u.Rol == "Administrador");

            var usuario = await _context.Usuarios.FindAsync(pago.UsuarioId);

            if (admin != null)
            {
                var nombreCliente = $"{usuario.Nombre} {usuario.Apellido}".Trim();

                // 🔔 Crear la notificación
                var notificacion = new Notificacion
                {
                    UsuarioId = admin.UsuarioId,
                    Titulo = "Nuevo pago registrado",
                    Mensaje = $"El cliente {nombreCliente} realizó un pago de S/. {pago.Monto:0.00} mediante {pago.MetodoPago}.",
                    Fecha = DateTime.Now,
                    Leido = false
                };

                _context.Notificaciones.Add(notificacion);
                await _context.SaveChangesAsync();
            }
        }
    }
    
}

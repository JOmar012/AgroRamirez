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
    public class PedidoDetallesController : Controller
    {
        private readonly AgropecuariaContext _context;

        public PedidoDetallesController(AgropecuariaContext context)
        {
            _context = context;
        }

        // GET: PedidoDetalles
        public async Task<IActionResult> Index()
        {
            var agropecuariaContext = _context.PedidoDetalles.Include(p => p.Pedido).ThenInclude(p => p.Usuario).Include(p => p.Producto);
            return View(await agropecuariaContext.ToListAsync());
        }

        // GET: PedidoDetalles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pedidoDetalle = await _context.PedidoDetalles
                .Include(p => p.Pedido)
                    .ThenInclude(p => p.Usuario)
                .Include(p => p.Producto)
                .FirstOrDefaultAsync(m => m.PedidoDetalleId == id);
            if (pedidoDetalle == null)
            {
                return NotFound();
            }

            return View(pedidoDetalle);
        }

        // GET: PedidoDetalles/Create
        public IActionResult Create()
        {
            ViewData["PedidoId"] = new SelectList(
                _context.Pedidos
                    .Include(p => p.Usuario)
                    .Select(p => new
                    {
                        p.PedidoId,
                        Texto = p.FechaPedido.ToString("dd/MM/yyyy") + " - "
                                + (p.Usuario.Nombre ?? "") + " " + (p.Usuario.Apellido ?? "")
                    }),
                "PedidoId",
                "Texto"
            );

            ViewData["ProductoId"] = new SelectList(_context.Productos, "ProductoId", "Nombre");

            return View(new PedidoDetalle());
        }

        // POST: PedidoDetalles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PedidoId,ProductoId,Cantidad,PrecioUnitario")] PedidoDetalle pedidoDetalle)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pedidoDetalle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["PedidoId"] = new SelectList(
                _context.Pedidos
                    .Include(p => p.Usuario)
                    .Select(p => new
                    {
                        p.PedidoId,
                        Texto = p.FechaPedido.ToString("dd/MM/yyyy") + " - " + p.Usuario.Nombre + " " + p.Usuario.Apellido
                    }),
                "PedidoId",
                "Texto",
                pedidoDetalle.PedidoId
            );

            ViewData["ProductoId"] = new SelectList(_context.Productos, "ProductoId", "Nombre", pedidoDetalle.ProductoId);
            return View(pedidoDetalle);
        }

        // GET: PedidoDetalles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pedidoDetalle = await _context.PedidoDetalles.FindAsync(id);
            if (pedidoDetalle == null)
            {
                return NotFound();
            }
            ViewData["PedidoId"] = new SelectList(
                _context.Pedidos
                    .Include(p => p.Usuario)
                    .Select(p => new
                    {
                        p.PedidoId,
                        Texto = p.FechaPedido.ToString("dd/MM/yyyy") + " - " + p.Usuario.Nombre + " " + p.Usuario.Apellido
                    }),
                "PedidoId",
                "Texto"
            );
            ViewData["ProductoId"] = new SelectList(_context.Productos, "ProductoId", "Nombre", pedidoDetalle.ProductoId);
            return View(pedidoDetalle);
        }

        // POST: PedidoDetalles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PedidoDetalle pedidoDetalle)
        {
            if (id != pedidoDetalle.PedidoDetalleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pedidoDetalle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PedidoDetalleExists(pedidoDetalle.PedidoDetalleId))
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
                 _context.Pedidos
                     .Include(p => p.Usuario)
                     .Select(p => new
                     {
                         p.PedidoId,
                         Texto = p.FechaPedido.ToString("dd/MM/yyyy") + " - " + p.Usuario.Nombre + " " + p.Usuario.Apellido
                     }),
                 "PedidoId",
                 "Texto"
             );
            ViewData["ProductoId"] = new SelectList(_context.Productos, "ProductoId", "Nombre", pedidoDetalle.ProductoId);
            return View(pedidoDetalle);
        }

        // GET: PedidoDetalles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pedidoDetalle = await _context.PedidoDetalles
                .Include(p => p.Pedido)
                    .ThenInclude(p => p.Usuario)
                .Include(p => p.Producto)
                .FirstOrDefaultAsync(m => m.PedidoDetalleId == id);
            if (pedidoDetalle == null)
            {
                return NotFound();
            }

            return View(pedidoDetalle);
        }

        // POST: PedidoDetalles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pedidoDetalle = await _context.PedidoDetalles.FindAsync(id);
            if (pedidoDetalle != null)
            {
                _context.PedidoDetalles.Remove(pedidoDetalle);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PedidoDetalleExists(int id)
        {
            return _context.PedidoDetalles.Any(e => e.PedidoDetalleId == id);
        }
    }
}

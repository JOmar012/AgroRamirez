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
        public IActionResult Create()
        {
            ViewData["PedidoId"] = new SelectList(
                _context.Pedidos.Select(p => new
                {
                    p.PedidoId,
                    Fecha = p.FechaPedido.ToString("dd/MM/yyyy")
                }),
                "PedidoId",
                "Fecha"
            );

            ViewData["UsuarioId"] = new SelectList(
                _context.Usuarios.Select(u => new
                {
                    u.UsuarioId,
                    NombreCompleto = u.Nombre + " " + u.Apellido
                }),
                "UsuarioId",
                "NombreCompleto"
            );

            return View();
        }

        // POST: Pagoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PagoId,PedidoId,UsuarioId,FechaPago,Monto,MetodoPago,Estado")] Pago pago)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pago);
                await _context.SaveChangesAsync();
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
    }
}

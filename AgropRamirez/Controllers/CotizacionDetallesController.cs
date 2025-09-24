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
    public class CotizacionDetallesController : Controller
    {
        private readonly AgropecuariaContext _context;

        public CotizacionDetallesController(AgropecuariaContext context)
        {
            _context = context;
        }

        // GET: CotizacionDetalles
        public async Task<IActionResult> Index()
        {
            var agropecuariaContext = _context.CotizacionDetalles.Include(c => c.Cotizacion).Include(c => c.Producto);
            return View(await agropecuariaContext.ToListAsync());
        }

        // GET: CotizacionDetalles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cotizacionDetalle = await _context.CotizacionDetalles
                .Include(c => c.Cotizacion)
                .Include(c => c.Producto)
                .FirstOrDefaultAsync(m => m.CotizacionDetalleId == id);
            if (cotizacionDetalle == null)
            {
                return NotFound();
            }

            return View(cotizacionDetalle);
        }

        // GET: CotizacionDetalles/Create
        public IActionResult Create()
        {
            ViewData["CotizacionId"] = new SelectList(
                _context.Cotizaciones
                .Select(c => new
                {
                    c.CotizacionId,
                    FechasT = c.FechaCotizacion + "hasta" + c.ValidoHasta
                }),
                "CotizacionId", "FechasT");

            ViewData["ProductoId"] = new SelectList(_context.Productos, "ProductoId", "Nombre");
            return View();
        }

        // POST: CotizacionDetalles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CotizacionDetalleId,CotizacionId,ProductoId,Cantidad,PrecioUnitario")] CotizacionDetalle cotizacionDetalle)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cotizacionDetalle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CotizacionId"] = new SelectList(
                _context.Cotizaciones
                .Select(c => new
                {
                    c.CotizacionId,
                    FechasT = c.FechaCotizacion + "hasta" + c.ValidoHasta
                }),
                "CotizacionId", "FechasT");

            ViewData["ProductoId"] = new SelectList(_context.Productos, "ProductoId", "Nombre", cotizacionDetalle.ProductoId);
            return View(cotizacionDetalle);
        }

        // GET: CotizacionDetalles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cotizacionDetalle = await _context.CotizacionDetalles.FindAsync(id);
            if (cotizacionDetalle == null)
            {
                return NotFound();
            }
            ViewData["CotizacionId"] = new SelectList(
                 _context.Cotizaciones
                 .Select(c => new
                 {
                     c.CotizacionId,
                     FechasT = c.FechaCotizacion + "hasta" + c.ValidoHasta
                 }),
                 "CotizacionId", "FechasT");

            ViewData["ProductoId"] = new SelectList(_context.Productos, "ProductoId", "Nombre", cotizacionDetalle.ProductoId);

            return View(cotizacionDetalle);
        }

        // POST: CotizacionDetalles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CotizacionDetalleId,CotizacionId,ProductoId,Cantidad,PrecioUnitario")] CotizacionDetalle cotizacionDetalle)
        {
            if (id != cotizacionDetalle.CotizacionDetalleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cotizacionDetalle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CotizacionDetalleExists(cotizacionDetalle.CotizacionDetalleId))
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
            ViewData["CotizacionId"] = new SelectList(
                _context.Cotizaciones
                .Select(c => new
                {
                    c.CotizacionId,
                    FechasT = c.FechaCotizacion + "hasta" + c.ValidoHasta
                }),
                "CotizacionId", "FechasT");

            ViewData["ProductoId"] = new SelectList(_context.Productos, "ProductoId", "Nombre", cotizacionDetalle.ProductoId);

            return View(cotizacionDetalle);
        }

        // GET: CotizacionDetalles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cotizacionDetalle = await _context.CotizacionDetalles
                .Include(c => c.Cotizacion)
                .Include(c => c.Producto)
                .FirstOrDefaultAsync(m => m.CotizacionDetalleId == id);
            if (cotizacionDetalle == null)
            {
                return NotFound();
            }

            return View(cotizacionDetalle);
        }

        // POST: CotizacionDetalles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cotizacionDetalle = await _context.CotizacionDetalles.FindAsync(id);
            if (cotizacionDetalle != null)
            {
                _context.CotizacionDetalles.Remove(cotizacionDetalle);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CotizacionDetalleExists(int id)
        {
            return _context.CotizacionDetalles.Any(e => e.CotizacionDetalleId == id);
        }
    }
}

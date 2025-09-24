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
    public class PromocionsController : Controller
    {
        private readonly AgropecuariaContext _context;

        public PromocionsController(AgropecuariaContext context)
        {
            _context = context;
        }

        // GET: Promocions
        public async Task<IActionResult> Index()
        {
            var agropecuariaContext = _context.Promociones.Include(p => p.Producto);
            return View(await agropecuariaContext.ToListAsync());
        }

        // GET: Promocions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var promocion = await _context.Promociones
                .Include(p => p.Producto)
                .FirstOrDefaultAsync(m => m.PromocionId == id);
            if (promocion == null)
            {
                return NotFound();
            }

            return View(promocion);
        }

        // GET: Promocions/Create
        public IActionResult Create()
        {
            ViewBag.Productos = _context.Productos.ToList();
            return View();
        }

        // POST: Promocions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Promocion promocion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(promocion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductoId"] = new SelectList(_context.Productos, "ProductoId", "Nombre", promocion.ProductoId);
            return View(promocion);
        }

        // GET: Promocions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var promocion = await _context.Promociones
                                          .Include(p => p.Producto) 
                                          .FirstOrDefaultAsync(p => p.PromocionId == id);

            if (promocion == null)
            {
                return NotFound();
            }

            ViewBag.Productos = _context.Productos.ToList();
            return View(promocion); 
        }

        // POST: Promocions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PromocionId,Nombre,Imagen,Descripcion,FechaInicio,FechaFin,Descuento,ProductoId")] Promocion promocion)
        {
            if (id != promocion.PromocionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(promocion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PromocionExists(promocion.PromocionId))
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
            ViewData["ProductoId"] = new SelectList(_context.Productos, "ProductoId", "Nombre", promocion.ProductoId);
            return View(promocion);
        }

        // GET: Promocions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var promocion = await _context.Promociones
                .Include(p => p.Producto)
                .FirstOrDefaultAsync(m => m.PromocionId == id);
            if (promocion == null)
            {
                return NotFound();
            }

            return View(promocion);
        }

        // POST: Promocions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var promocion = await _context.Promociones.FindAsync(id);
            if (promocion != null)
            {
                _context.Promociones.Remove(promocion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PromocionExists(int id)
        {
            return _context.Promociones.Any(e => e.PromocionId == id);
        }
    }
}

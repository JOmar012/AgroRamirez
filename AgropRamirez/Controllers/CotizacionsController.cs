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
    public class CotizacionsController : Controller
    {
        private readonly AgropecuariaContext _context;

        public CotizacionsController(AgropecuariaContext context)
        {
            _context = context;
        }

        // GET: Cotizacions
        public async Task<IActionResult> Index()
        {
            var agropecuariaContext = _context.Cotizaciones.Include(c => c.Usuario);
            return View(await agropecuariaContext.ToListAsync());
        }

        // GET: Cotizacions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cotizacion = await _context.Cotizaciones
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(m => m.CotizacionId == id);
            if (cotizacion == null)
            {
                return NotFound();
            }

            return View(cotizacion);
        }

        // GET: Cotizacions/Create
        public IActionResult Create()
        {
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios
                .Select(c => new
                {
                    c.UsuarioId,
                    NomCompleto = c.Nombre + " " + c.Apellido
                }
                ),
                
                "UsuarioId", "NomCompleto");
            return View();
        }

        // POST: Cotizacions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CotizacionId,UsuarioId,FechaCotizacion,ValidoHasta,Total,Estado")] Cotizacion cotizacion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cotizacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios
                .Select(c => new
                {
                    c.UsuarioId,
                    NomCompleto = c.Nombre + " " + c.Apellido
                }
                ),

                "UsuarioId", "NomCompleto");
            return View(cotizacion);
        }

        // GET: Cotizacions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cotizacion = await _context.Cotizaciones.FindAsync(id);
            if (cotizacion == null)
            {
                return NotFound();
            }
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios
                 .Select(c => new
                 {
                     c.UsuarioId,
                     NomCompleto = c.Nombre + " " + c.Apellido
                 }
                 ),

                 "UsuarioId", "NomCompleto");
            return View(cotizacion);
        }

        // POST: Cotizacions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CotizacionId,UsuarioId,FechaCotizacion,ValidoHasta,Total,Estado")] Cotizacion cotizacion)
        {
            if (id != cotizacion.CotizacionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cotizacion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CotizacionExists(cotizacion.CotizacionId))
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
                .Select(c => new
                {
                    c.UsuarioId,
                    NomCompleto = c.Nombre + " " + c.Apellido
                }
                ),

                "UsuarioId", "NomCompleto");
            return View(cotizacion);
        }

        // GET: Cotizacions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cotizacion = await _context.Cotizaciones
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(m => m.CotizacionId == id);
            if (cotizacion == null)
            {
                return NotFound();
            }

            return View(cotizacion);
        }

        // POST: Cotizacions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cotizacion = await _context.Cotizaciones.FindAsync(id);
            if (cotizacion != null)
            {
                _context.Cotizaciones.Remove(cotizacion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CotizacionExists(int id)
        {
            return _context.Cotizaciones.Any(e => e.CotizacionId == id);
        }
    }
}

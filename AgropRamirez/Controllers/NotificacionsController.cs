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
    public class NotificacionsController : Controller
    {
        private readonly AgropecuariaContext _context;

        public NotificacionsController(AgropecuariaContext context)
        {
            _context = context;
        }

        // GET: Notificacions
        public async Task<IActionResult> Index()
        {
            var agropecuariaContext = _context.Notificaciones.Include(n => n.Usuario);
            return View(await agropecuariaContext.ToListAsync());
        }

        // GET: Notificacions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notificacion = await _context.Notificaciones
                .Include(n => n.Usuario)
                .FirstOrDefaultAsync(m => m.NotificacionId == id);
            if (notificacion == null)
            {
                return NotFound();
            }

            return View(notificacion);
        }

        // GET: Notificacions/Create
        public IActionResult Create()
        {

            ViewData["UsuarioId"] = new SelectList(_context.Usuarios
                .Select(n => new {
                    n.UsuarioId,
                    NombreCompleto = n.Nombre + " " + n.Apellido
                }),
                "UsuarioId", "NombreCompleto");

            return View();
        }

        // POST: Notificacions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NotificacionId,UsuarioId,Titulo,Mensaje,Fecha,Leido")] Notificacion notificacion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(notificacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios
                .Select(n => new {
                    n.UsuarioId,
                    NombreCompleto = n.Nombre + " " + n.Apellido
                }),
                "UsuarioId", "NombreCompleto");

            return View(notificacion);
        }

        // GET: Notificacions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notificacion = await _context.Notificaciones.FindAsync(id);
            if (notificacion == null)
            {
                return NotFound();
            }

            ViewData["UsuarioId"] = new SelectList(_context.Usuarios
                .Select(n => new {
                    n.UsuarioId,
                    NombreCompleto = n.Nombre + " " + n.Apellido
                }),
                "UsuarioId", "NombreCompleto");

            return View(notificacion);
        }

        // POST: Notificacions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NotificacionId,UsuarioId,Titulo,Mensaje,Fecha,Leido")] Notificacion notificacion)
        {
            if (id != notificacion.NotificacionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(notificacion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NotificacionExists(notificacion.NotificacionId))
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
                .Select(n => new {
                    n.UsuarioId,
                    NombreCompleto = n.Nombre + " " + n.Apellido
                }),
                "UsuarioId", "NombreCompleto");

            return View(notificacion);
        }

        // GET: Notificacions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notificacion = await _context.Notificaciones
                .Include(n => n.Usuario)
                .FirstOrDefaultAsync(m => m.NotificacionId == id);
            if (notificacion == null)
            {
                return NotFound();
            }

            return View(notificacion);
        }

        // POST: Notificacions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var notificacion = await _context.Notificaciones.FindAsync(id);
            if (notificacion != null)
            {
                _context.Notificaciones.Remove(notificacion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NotificacionExists(int id)
        {
            return _context.Notificaciones.Any(e => e.NotificacionId == id);
        }
    }
}

using AgropRamirez.Data;
using AgropRamirez.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
    [Authorize]
    public class NotificacionsController : Controller
    {
        private readonly AgropecuariaContext _context;


        public NotificacionsController(AgropecuariaContext context)
        {
            _context = context;

        }

        // GET: Notificacions
        public async Task<IActionResult> Index(bool soloPropias = false)
        {
            // ✅ Obtener el claim correcto
            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var rolClaim = User.FindFirst(ClaimTypes.Role)?.Value;

            int.TryParse(usuarioIdClaim, out int usuarioId);
            bool esAdmin = rolClaim == "Administrador";

            // 🔹 Base de consulta
            var notificaciones = _context.Notificaciones
                .Include(n => n.Usuario)
                .OrderByDescending(n => n.Fecha)
                .AsQueryable();

            // 🔸 Filtrar según el rol
            if (!esAdmin)
            {
                // Cliente o Empleado → solo sus notificaciones
                notificaciones = notificaciones.Where(n => n.UsuarioId == usuarioId);
            }
            else if (soloPropias)
            {
                // Admin activó el filtro “solo mis notificaciones”
                notificaciones = notificaciones.Where(n => n.UsuarioId == usuarioId);
            }

            // 🔹 Datos para la vista
            ViewBag.EsAdmin = esAdmin;
            ViewBag.SoloPropias = soloPropias;

            return View(await notificaciones.ToListAsync());
        
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

            // ✅ Marcar como leída si no lo está
            if (!notificacion.Leido)
            {
                notificacion.Leido = true;
                _context.Update(notificacion);
                await _context.SaveChangesAsync();
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

        //Notificacion en el nabvar
        [Authorize]
        public async Task<PartialViewResult> NotificacionesNavbar()
        {
            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // ✅ Si el usuario no está logueado o no hay claim, devolver lista vacía
            if (string.IsNullOrEmpty(usuarioIdClaim))
                return PartialView("_NotificacionesNavbar", new List<Notificacion>());

            int.TryParse(usuarioIdClaim, out int usuarioId);

            var notificaciones = await _context.Notificaciones
                .Where(n => n.UsuarioId == usuarioId)
                .OrderByDescending(n => n.Fecha)
                .Take(5)
                .ToListAsync();

            // ✅ Si no hay registros, devolver lista vacía (no null)
            return PartialView("_NotificacionesNavbar", notificaciones ?? new List<Notificacion>());
        }

        [HttpPost]
        public async Task<IActionResult> CambiarEstado(int id, bool leido)
        {
            var notificacion = await _context.Notificaciones.FindAsync(id);
            if (notificacion == null)
                return NotFound();

            notificacion.Leido = leido;
            _context.Update(notificacion);
            await _context.SaveChangesAsync();

            // Contar las no leídas
            var usuarioId = notificacion.UsuarioId;
            var noLeidas = await _context.Notificaciones
                .Where(n => n.UsuarioId == usuarioId && !n.Leido)
                .CountAsync();

            return Json(new { success = true, noLeidas });
        }
    }
}

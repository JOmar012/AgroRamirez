using AgropRamirez.Data;
using AgropRamirez.Models;
using AgropRamirez.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgropRamirez.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UsuariosController : Controller
    {
        private readonly AgropecuariaContext _context;
        private readonly IWebHostEnvironment _env;//img

        public UsuariosController(AgropecuariaContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            return View(await _context.Usuarios.ToListAsync());
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.UsuarioId == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/Create
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            ViewBag.Roles = new List<string> { "Administrador", "Empleado", "Cliente" };
            return View();
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create(UsuarioCreateVM vm)
        {

            if (!ModelState.IsValid)
            {
                // Si el modelo no es válido, regresa la vista con errores
                ViewBag.Roles = new List<string> { "Administrador", "Empleado", "Cliente" };
                return View(vm);
            }

            if (string.IsNullOrWhiteSpace(vm.Password))
            {
                ModelState.AddModelError("Password", "La contraseña es obligatoria.");
                ViewBag.Roles = new List<string> { "Administrador", "Empleado", "Cliente" };
                return View(vm);
            }

            string? rutaImagen = null;
            if (vm.ImagenFile != null && vm.ImagenFile.Length > 0)
            {
                var uploads = Path.Combine(_env.WebRootPath, "img", "usuarios");
                Directory.CreateDirectory(uploads);

                var fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(vm.ImagenFile.FileName);
                var fullPath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await vm.ImagenFile.CopyToAsync(stream);
                }

                rutaImagen = $"/img/usuarios/{fileName}";
            }

            var nuevoUsuario = new Usuario
            {
                Nombre = vm.Nombre,
                Apellido = vm.Apellido,
                Dni = vm.Dni,
                Direccion = vm.Direccion,
                Imagen = rutaImagen,
                Email = vm.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(vm.Password),
                Rol = vm.Rol,
                FechaRegistro = DateTime.Now
            };

            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            var vm = new UsuarioCreateVM
            {
                UsuarioId = usuario.UsuarioId,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Dni = usuario.Dni,
                Direccion = usuario.Direccion,
                Imagen = usuario.Imagen,
                Email = usuario.Email,
                Rol = usuario.Rol
            };

            ViewBag.Roles = new List<string> { "Administrador", "Empleado", "Cliente" };
            return View(vm);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UsuarioCreateVM vm)
        {
            if (id != vm.UsuarioId) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Roles = new List<string> { "Administrador", "Empleado", "Cliente" };
                return View(vm);
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            // Actualizar datos básicos
            usuario.Nombre = vm.Nombre;
            usuario.Apellido = vm.Apellido;
            usuario.Dni = vm.Dni;
            usuario.Direccion = vm.Direccion;
            usuario.Email = vm.Email;
            usuario.Rol = vm.Rol;

            // Solo actualiza la contraseña si se ingresó una nueva
            if (!string.IsNullOrWhiteSpace(vm.Password))
            {
                usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(vm.Password);
            }

            // Procesar imagen si se cargó una nueva
            if (vm.ImagenFile != null && vm.ImagenFile.Length > 0)
            {
                var uploads = Path.Combine(_env.WebRootPath, "img", "usuarios");
                Directory.CreateDirectory(uploads);

                var fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(vm.ImagenFile.FileName);
                var fullPath = Path.Combine(uploads, fileName);

                using var stream = new FileStream(fullPath, FileMode.Create);
                await vm.ImagenFile.CopyToAsync(stream);

                usuario.Imagen = $"/img/usuarios/{fileName}";
            }

            _context.Update(usuario);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.UsuarioId == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.UsuarioId == id);
        }
    }
}

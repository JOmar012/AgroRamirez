using AgropRamirez.Data;
using AgropRamirez.Models;
using AgropRamirez.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<IActionResult> Index(
                                        string busqueda,
                                        int? descuentoMin,
                                        DateTime? fechaInicio,
                                        DateTime? fechaFin,
                                        int pagina = 1)
        {
            int cantidadPorPagina = 8;

            var query = _context.Promociones
                .Include(p => p.Productos)
                .AsQueryable();

            // 🔍 Filtro por nombre
            if (!string.IsNullOrEmpty(busqueda))
            {
                busqueda = busqueda.ToLower();
                query = query.Where(p => p.Nombre.ToLower().Contains(busqueda));
            }

            // 🎯 Filtro por descuento mínimo
            if (descuentoMin.HasValue)
            {
                query = query.Where(p => p.Descuento >= descuentoMin.Value);
            }

            // 📅 Filtro por rango de fechas
            if (fechaInicio.HasValue)
            {
                query = query.Where(p => p.FechaInicio >= fechaInicio.Value);
            }

            if (fechaFin.HasValue)
            {
                query = query.Where(p => p.FechaFin <= fechaFin.Value);
            }

            int totalPromos = await query.CountAsync();

            var promos = await query
                .OrderByDescending(p => p.FechaInicio)
                .Skip((pagina - 1) * cantidadPorPagina)
                .Take(cantidadPorPagina)
                .ToListAsync();

            var vm = new PromocionesIndexViewModel
            {
                Promociones = promos,
                Busqueda = busqueda,
                DescuentoMin = descuentoMin,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                PaginaActual = pagina,
                TotalPaginas = (int)Math.Ceiling(totalPromos / (double)cantidadPorPagina)
            };

            return View(vm);
        }

        // GET: Promocions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var promocion = await _context.Promociones
                                          .Include(p => p.Productos)
                                          .FirstOrDefaultAsync(m => m.PromocionId == id);
            if (promocion == null) return NotFound();

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
        public async Task<IActionResult> Create(PromocionCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var promo = new Promocion
                {
                    Nombre = model.Nombre,
                    Descripcion = model.Descripcion,
                    FechaInicio = model.FechaInicio,
                    FechaFin = model.FechaFin,
                    Descuento = model.Descuento,
                    Imagen = GuardarImagen(model.ImagenFile) // tu helper para guardar imágenes
                };

                // Asignar productos
                promo.Productos = _context.Productos
                                          .Where(p => model.ProductosSeleccionados.Contains(p.ProductoId))
                                          .ToList();

                _context.Promociones.Add(promo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Productos = _context.Productos.ToList();
            return View(model);
        }

        // GET: Promocions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var promo = await _context.Promociones
                                      .Include(p => p.Productos)
                                      .FirstOrDefaultAsync(p => p.PromocionId == id);
            if (promo == null) return NotFound();

            var vm = new PromocionCreateViewModel
            {
                PromocionId = promo.PromocionId,
                Nombre = promo.Nombre,
                Descripcion = promo.Descripcion,
                FechaInicio = promo.FechaInicio,
                FechaFin = promo.FechaFin,
                Descuento = promo.Descuento,
                ProductosSeleccionados = promo.Productos.Select(p => p.ProductoId).ToList()
            };

            ViewBag.Productos = _context.Productos.ToList();
            return View(vm);
        }

        // POST: Promocions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PromocionCreateViewModel model)
        {
            if (id != model.PromocionId) return NotFound();

            if (ModelState.IsValid)
            {
                var promo = await _context.Promociones
                                          .Include(p => p.Productos)
                                          .FirstOrDefaultAsync(p => p.PromocionId == id);
                if (promo == null) return NotFound();

                promo.Nombre = model.Nombre;
                promo.Descripcion = model.Descripcion;
                promo.FechaInicio = model.FechaInicio;
                promo.FechaFin = model.FechaFin;
                promo.Descuento = model.Descuento;
                if (model.ImagenFile != null)
                    promo.Imagen = GuardarImagen(model.ImagenFile);

                // Actualizar productos asociados
                promo.Productos.Clear();
                promo.Productos = _context.Productos
                                          .Where(p => model.ProductosSeleccionados.Contains(p.ProductoId))
                                          .ToList();

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Productos = _context.Productos.ToList();
            return View(model);
        }

        // GET: Promocions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var promocion = await _context.Promociones
                                          .Include(p => p.Productos)
                                          .FirstOrDefaultAsync(m => m.PromocionId == id);
            if (promocion == null) return NotFound();

            return View(promocion);
        }

        // POST: Promocions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var promocion = await _context.Promociones
                                          .Include(p => p.Productos)
                                          .FirstOrDefaultAsync(p => p.PromocionId == id);

            if (promocion != null)
            {
                _context.Promociones.Remove(promocion);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        //Guardar imagen
        private string? GuardarImagen(IFormFile? imagenFile)
        {
            if (imagenFile == null || imagenFile.Length == 0)
                return null;

            // Carpeta donde se guardarán las imágenes
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/promociones");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Nombre único para la imagen
            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imagenFile.FileName);

            // Ruta completa en el servidor
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Guardar archivo
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                imagenFile.CopyTo(stream);
            }

            // Retorna la ruta relativa que guardarás en la BD
            return "/img/promociones/" + uniqueFileName;
        }

        private bool PromocionExists(int id)
        {
            return _context.Promociones.Any(e => e.PromocionId == id);
        }
    }
}

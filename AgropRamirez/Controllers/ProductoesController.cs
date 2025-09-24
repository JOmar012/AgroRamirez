using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AgropRamirez.Data;
using AgropRamirez.Models;
using Microsoft.AspNetCore.Hosting;

namespace AgropRamirez.Controllers
{
    public class ProductoesController : Controller
    {
        private readonly AgropecuariaContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductoesController(AgropecuariaContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Productoes
        public async Task<IActionResult> Index()
        {
            var agropecuariaContext = _context.Productos.Include(p => p.Categoria);
            return View(await agropecuariaContext.ToListAsync());
        }

        // GET: Productoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.ProductoId == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // GET: Productoes/Create
        public IActionResult Create()
        {
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "CategoriaId", "Nombre");
            return View();
        }

        // POST: Productoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( Producto producto)
        {
            // Quitamos validación de la propiedad de navegación
            //ModelState.Remove("Categoria");

            if (ModelState.IsValid)
            {
                // Manejo de imagen
                if (producto.ImagenFile != null)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(producto.ImagenFile.FileName);
                    string path = Path.Combine(wwwRootPath, "img/productos", fileName);

                    // Crear carpeta si no existe
                    if (!Directory.Exists(Path.Combine(wwwRootPath, "img/productos")))
                    {
                        Directory.CreateDirectory(Path.Combine(wwwRootPath, "img/productos"));
                    }

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await producto.ImagenFile.CopyToAsync(fileStream);
                    }

                    // Guardamos la ruta en la BD
                    producto.Imagen = "/img/productos/" + fileName;
                }

                _context.Add(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                // Log de errores
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine("Error: " + error.ErrorMessage);
                }
            }

            // En caso de error volvemos a llenar el combo
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "CategoriaId", "Nombre", producto.CategoriaId);
            return View(producto);
        }

        // GET: Productoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "CategoriaId", "Nombre", producto.CategoriaId);
            return View(producto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]    
        public async Task<IActionResult> Edit(int id, Producto producto)
        {
            if (id != producto.ProductoId)      
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var productoBD = await _context.Productos.AsNoTracking().FirstOrDefaultAsync(p => p.ProductoId == id);

                    if (productoBD == null)
                    {
                        return NotFound();
                    }

                    // Manejo de imagen
                    if (producto.ImagenFile != null)
                    {
                        string wwwRootPath = _webHostEnvironment.WebRootPath;
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(producto.ImagenFile.FileName);
                        string path = Path.Combine(wwwRootPath, "img/productos", fileName);

                        if (!Directory.Exists(Path.Combine(wwwRootPath, "img/productos")))
                        {
                            Directory.CreateDirectory(Path.Combine(wwwRootPath, "img/productos"));
                        }

                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await producto.ImagenFile.CopyToAsync(fileStream);
                        }

                        // Reemplazar ruta de imagen
                        producto.Imagen = "/img/productos/" + fileName;
                    }
                    else
                    {
                        // Si no subió nueva, conservar la existente
                        producto.Imagen = productoBD.Imagen;
                    }

                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(producto.ProductoId))
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

            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "CategoriaId", "Nombre", producto.CategoriaId);
            return View(producto);
        }

        // GET: Productoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.ProductoId == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // POST: Productoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null)
            {
                _context.Productos.Remove(producto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.ProductoId == id);
        }
    }
}

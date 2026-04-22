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
    public class CarritoDetallesController : Controller
    {
        private readonly AgropecuariaContext _context;

        public CarritoDetallesController(AgropecuariaContext context)
        {
            _context = context;
        }

        // GET: CarritoDetalles
        public async Task<IActionResult> Index()
        {
            var agropecuariaContext = _context.CarritoDetalles.Include(c => c.Carrito).ThenInclude(c => c.Usuario).Include(c => c.Producto);
            return View(await agropecuariaContext.ToListAsync());
        }

        // GET: CarritoDetalles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carritoDetalle = await _context.CarritoDetalles
                .Include(c => c.Carrito)
                    .ThenInclude(c => c.Usuario)
                .Include(c => c.Producto)
                .FirstOrDefaultAsync(m => m.CarritoDetalleId == id);
            if (carritoDetalle == null)
            {
                return NotFound();
            }

            return View(carritoDetalle);
        }

        // GET: CarritoDetalles/Create
        public IActionResult Create()
        {

                ViewData["CarritoId"] = new SelectList(
                    _context.Carritos
                        .Include(c => c.Usuario) 
                        .Select(c => new
                        {
                            c.CarritoId,
                            Texto = c.FechaCreacion.ToString("dd/MM/yyyy") + " - " + c.Usuario.Nombre + " " + c.Usuario.Apellido
                        }),
                    "CarritoId",
                    "Texto"
                );

            ViewData["ProductoId"] = new SelectList(_context.Productos, "ProductoId", "Nombre");
            return View();
        }

        // POST: CarritoDetalles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CarritoDetalleId,CarritoId,ProductoId,Cantidad,PrecioUnitario")] CarritoDetalle carritoDetalle)
        {
            if (ModelState.IsValid)
            {
                _context.Add(carritoDetalle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CarritoId"] = new SelectList(
                    _context.Carritos
                        .Include(c => c.Usuario)
                        .Select(c => new
                        {
                            c.CarritoId,
                            Texto = c.FechaCreacion.ToString("dd/MM/yyyy") + " - " + c.Usuario.Nombre + " " + c.Usuario.Apellido
                        }),
                    "CarritoId",
                    "Texto"
                );
            ViewData["ProductoId"] = new SelectList(_context.Productos, "ProductoId", "Nombre", carritoDetalle.ProductoId);
            return View(carritoDetalle);
        }

        // GET: CarritoDetalles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carritoDetalle = await _context.CarritoDetalles.FindAsync(id);
            if (carritoDetalle == null)
            {
                return NotFound();
            }
            ViewData["CarritoId"] = new SelectList(
                    _context.Carritos
                        .Include(c => c.Usuario)
                        .Select(c => new
                        {
                            c.CarritoId,
                            Texto = c.FechaCreacion.ToString("dd/MM/yyyy") + " - " + c.Usuario.Nombre + " " + c.Usuario.Apellido
                        }),
                    "CarritoId",
                    "Texto"
            );
            ViewData["ProductoId"] = new SelectList(_context.Productos, "ProductoId", "Nombre", carritoDetalle.ProductoId);
            return View(carritoDetalle);
        }

        // POST: CarritoDetalles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CarritoDetalleId,CarritoId,ProductoId,Cantidad,PrecioUnitario")] CarritoDetalle carritoDetalle)
        {
            if (id != carritoDetalle.CarritoDetalleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(carritoDetalle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarritoDetalleExists(carritoDetalle.CarritoDetalleId))
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
            ViewData["CarritoId"] = new SelectList(
                    _context.Carritos
                        .Include(c => c.Usuario)
                        .Select(c => new
                        {
                            c.CarritoId,
                            Texto = c.FechaCreacion.ToString("dd/MM/yyyy") + " - " + c.Usuario.Nombre + " " + c.Usuario.Apellido
                        }),
                    "CarritoId",
                    "Texto"
                );
            ViewData["ProductoId"] = new SelectList(_context.Productos, "ProductoId", "Nombre", carritoDetalle.ProductoId);
            return View(carritoDetalle);
        }

        // GET: CarritoDetalles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carritoDetalle = await _context.CarritoDetalles
                .Include(c => c.Carrito)
                    .ThenInclude(c => c.Usuario)
                .Include(c => c.Producto)
                .FirstOrDefaultAsync(m => m.CarritoDetalleId == id);
            if (carritoDetalle == null)
            {
                return NotFound();
            }

            return View(carritoDetalle);
        }

        // POST: CarritoDetalles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var carritoDetalle = await _context.CarritoDetalles.FindAsync(id);
            if (carritoDetalle != null)
            {
                _context.CarritoDetalles.Remove(carritoDetalle);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarritoDetalleExists(int id)
        {
            return _context.CarritoDetalles.Any(e => e.CarritoDetalleId == id);
        }
    }
}

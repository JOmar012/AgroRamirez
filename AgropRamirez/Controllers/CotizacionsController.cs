using AgropRamirez.Data;
using AgropRamirez.Models;
using AgropRamirez.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Xml.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

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
            
                var cotizaciones = await _context.Cotizaciones
                    .Include(c => c.Usuario)
                    .Include(c => c.CotizacionDetalles)               // 🔹 Incluye los detalles
                        .ThenInclude(d => d.Producto)                 // 🔹 Incluye los productos de cada detalle
                    .OrderByDescending(c => c.FechaCotizacion)        // 🔹 Ordenar de la más reciente a la más antigua
                    .ToListAsync();

                return View(cotizaciones);
            
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
                .Include(c => c.CotizacionDetalles)               // 🔹 Incluye los detalles
                        .ThenInclude(d => d.Producto)
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

            var hoy = DateTime.Now.Date;
            var hasta = hoy.AddDays(5);

            var vm = new CotizacionCreateViewModel
            {
                Cotizacion = new Cotizacion
                {
                    FechaCotizacion = hoy,
                    ValidoHasta = hasta,
                    Estado = "Pendiente"
                }
            };

            if (User.IsInRole("Administrador"))
            {
                ViewBag.EsAdmin = true;
                ViewBag.Usuarios = _context.Usuarios
                    .Select(u => new
                    {
                        u.UsuarioId,
                        NombreCompleto = u.Nombre + " " + u.Apellido
                    })
                    .ToList();
            }
            else
            {
                ViewBag.EsAdmin = false;
            }

            // 👇 Enviamos todos los campos relevantes al JS
            var productos = _context.Productos
                .Select(p => new
                {
                    p.ProductoId,
                    p.Nombre,
                    p.Precio,
                    p.PrecioPorMayor,
                    p.UmbralMayorista,
                    p.Stock
                })
                .ToList();

            var jsonOpts = new System.Text.Json.JsonSerializerOptions 
            { 
                PropertyNamingPolicy = null 
            };
            ViewBag.ProductosJson = System.Text.Json.JsonSerializer.Serialize(productos, jsonOpts);

            return View(vm);
        }

        // POST: Cotizacions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CotizacionCreateViewModel vm)
        {
            // 🔍 LÍNEA DE DEPURACIÓN — imprime en la consola del servidor
            Console.WriteLine($"Detalles recibidos: {vm.Detalles?.Count ?? 0}");

            int userId; 
            if (User.IsInRole("Administrador"))
            { // Si es admin, toma el usuario del formulario
              userId = vm.Cotizacion.UsuarioId; } 
            else
            { // Si es cliente, toma el ID del usuario logueado
              userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value); 
            } 
            vm.Cotizacion.UsuarioId = userId;

            // ✅ Normalizar precios antes de guardar
            foreach (var det in vm.Detalles)
            {
                det.PrecioUnitario = det.PrecioUnitario / 100; // 🔥 CORRECCIÓN CLAVE
            }

            vm.Cotizacion.Total = vm.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario);
            
            if (ModelState.IsValid) 
            { 
                _context.Add(vm.Cotizacion); 
                await _context.SaveChangesAsync(); 
                
                foreach (var det in vm.Detalles) 
                { 
                    det.CotizacionId = vm.Cotizacion.CotizacionId; _context.Add(det); 
                }
                await _context.SaveChangesAsync(); return RedirectToAction(nameof(Index)); 
            } 
            // Volver a llenar combos si hay error
              ViewBag.EsAdmin = User.IsInRole("Administrador"); 
            if (ViewBag.EsAdmin) 
            { ViewBag.Usuarios = _context.Usuarios 
                    .Select(u => new 
                    {
                        u.UsuarioId, NombreCompleto = u.Nombre + " " + u.Apellido 
                    }) 
                    .ToList(); 
            } 
            
            ViewBag.Productos = _context.Productos 
                .Select(p => new { p.ProductoId, p.Nombre, p.Precio }) 
                .ToList(); return View(vm);
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
                .Include(c => c.CotizacionDetalles)             // 🔹 Incluye los detalles
                        .ThenInclude(d => d.Producto)
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


        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> GenerarPdf([FromBody] CotizacionTempDto data)
        {
            // 1️⃣ Si viene un ID (cotización guardada en BD)
            if (data.CotizacionId.HasValue)
            {
                // Carga la cotización desde la base de datos
                var cotizacion = await _context.Cotizaciones
                    .Include(c => c.Usuario)
                    .Include(c => c.CotizacionDetalles)
                        .ThenInclude(d => d.Producto)
                    .FirstOrDefaultAsync(c => c.CotizacionId == data.CotizacionId.Value);

                if (cotizacion == null)
                    return NotFound();

                // Llena los datos desde la BD
                data.Cliente = $"{cotizacion.Usuario.Nombre} {cotizacion.Usuario.Apellido}";
                data.Fecha = cotizacion.FechaCotizacion.ToString("dd/MM/yyyy");
                data.VigenteHasta = cotizacion.ValidoHasta?.ToString("dd/MM/yyyy") ?? "";
                data.Total = cotizacion.Total.ToString("0.00");
                data.Detalles = cotizacion.CotizacionDetalles.Select(d => new DetalleTempDto
                {
                    Producto = d.Producto.Nombre,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario
                }).ToList();
            }

            // 2️⃣ Si NO viene un ID, es una cotización temporal
            // (visitante no logueado)
            // Los datos ya vienen desde el front en "data.Detalles", "data.Total", etc.

            // 📄 Construcción del PDF (igual para ambos casos)
            using var ms = new MemoryStream();
            using var doc = new iTextSharp.text.Document(PageSize.A4, 40, 40, 40, 40);
            PdfWriter.GetInstance(doc, ms);
            doc.Open();

            var titulo = new Paragraph("COTIZACIÓN", new Font(Font.FontFamily.HELVETICA, 16, Font.BOLD))
            {
                Alignment = Element.ALIGN_CENTER
            };
            doc.Add(titulo);
            doc.Add(new Paragraph($"Cliente: {data.Cliente ?? "Cotización sin registro"}"));
            doc.Add(new Paragraph($"Fecha: {data.Fecha}"));
            if (!string.IsNullOrEmpty(data.VigenteHasta))
                doc.Add(new Paragraph($"Válido hasta: {data.VigenteHasta}"));
            doc.Add(new Paragraph(" "));

            var tabla = new PdfPTable(4) { WidthPercentage = 100 };
            tabla.AddCell("Producto");
            tabla.AddCell("Cantidad");
            tabla.AddCell("Precio Unitario (S/)");
            tabla.AddCell("Subtotal (S/)");
            foreach (var d in data.Detalles)
            {
                tabla.AddCell(d.Producto);
                tabla.AddCell(d.Cantidad.ToString());
                tabla.AddCell(d.PrecioUnitario.ToString("0.00"));
                tabla.AddCell((d.Cantidad * d.PrecioUnitario).ToString("0.00"));
            }
            doc.Add(tabla);

            doc.Add(new Paragraph(" "));
            doc.Add(new Paragraph($"Total: S/ {data.Total}", new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD)));

            doc.Add(new Paragraph(" "));
            doc.Add(new Paragraph("Cotización válida por 5 días desde la fecha de emisión.", new Font(Font.FontFamily.HELVETICA, 10, Font.ITALIC)));

            doc.Close();

            return File(ms.ToArray(), "application/pdf", $"Cotizacion_{DateTime.Now:yyyyMMddHHmmss}.pdf");
        }

    }


}

using AgropRamirez.Data;
using AgropRamirez.Models;
using AgropRamirez.ViewModels;
using AgropRamirez.ViewModels.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using System.Security.Claims;

namespace AgropRamirez.Controllers
{
    public class CuentaController : Controller
    {
        private readonly AgropecuariaContext _context;
        private readonly IWebHostEnvironment _env;

        public CuentaController(AgropecuariaContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ============================================
        // GET: /Cuenta/Login
        // ============================================
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            return View(new LoginVM { ReturnUrl = returnUrl });
        }

        // ============================================
        // POST: /Cuenta/Login
        // ============================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == vm.Email);
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(vm.Password, usuario.PasswordHash))
            {
                ModelState.AddModelError(string.Empty, "Correo o contraseña inválidos.");
                return View(vm);
            }

            // Crear lista de claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioId.ToString()),
                new Claim(ClaimTypes.Name, $"{usuario.Nombre} {usuario.Apellido}"),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Rol)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // Redirección tras login
            if (!string.IsNullOrWhiteSpace(vm.ReturnUrl) && Url.IsLocalUrl(vm.ReturnUrl))
                return Redirect(vm.ReturnUrl);

            return RedirectToAction("Index", "Home");
        }

        // ============================================
        // GET: /Cuenta/Registrar
        // ============================================
        [HttpGet]
        public IActionResult Registrar()
        {
            return View(new RegistrarVM());
        }

        // ============================================
        // POST: /Cuenta/Registrar
        // ============================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrar(RegistrarVM vm)
        {
            if (!ModelState.IsValid) return View(vm);

            if (_context.Usuarios.Any(u => u.Email == vm.Email))
            {
                ModelState.AddModelError(nameof(vm.Email), "Este correo ya está registrado.");
                return View(vm);
            }

            string? rutaImagen = null;
            if (vm.ImagenFile != null && vm.ImagenFile.Length > 0)
            {
                var uploads = Path.Combine(_env.WebRootPath, "uploads", "usuarios");
                Directory.CreateDirectory(uploads);

                var fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(vm.ImagenFile.FileName);
                var path = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await vm.ImagenFile.CopyToAsync(stream);
                }

                rutaImagen = $"/uploads/usuarios/{fileName}";
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
                Rol = "Cliente",
                FechaRegistro = DateTime.Now
            };

            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync();

            // Iniciar sesión tras registro
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, nuevoUsuario.UsuarioId.ToString()),
                new Claim(ClaimTypes.Name, $"{nuevoUsuario.Nombre} {nuevoUsuario.Apellido}"),
                new Claim(ClaimTypes.Email, nuevoUsuario.Email),
                new Claim(ClaimTypes.Role, nuevoUsuario.Rol)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }

        // ============================================
        // /Cuenta/Logout
        // ============================================
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        // ============================================
        // /Cuenta/AccesoDenegado
        // ============================================
        [HttpGet]
        public IActionResult AccesoDenegado()
        {
            return View();
        }
    }
}

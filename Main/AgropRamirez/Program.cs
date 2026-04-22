using AgropRamirez.Data; // DbContext
using AgropRamirez.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// 1 Configurar la cadena de conexión desde appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2 Registrar DbContext con SQL Server
builder.Services.AddDbContext<AgropecuariaContext>(options =>
    options.UseSqlServer(connectionString));


// 3 Agregar servicios MVC
builder.Services.AddControllersWithViews();

// 4 Habilitar sesiones (opcional, útil para carrito)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60); // Duración de la sesión
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 5 Configurar autenticación por cookies 
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Cuenta/Login";
        options.AccessDeniedPath = "/Cuenta/AccesoDenegado";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// 6. Configuración del pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();          // <-- Si usas sesión (carrito, etc.)
app.UseAuthentication(); // <- si usas login
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Llama al método del crear admin:
await SeedAdminUserAsync(app.Services, app.Configuration);


app.Run();

// Método para sembrar el administrador por defecto
async Task SeedAdminUserAsync(IServiceProvider services, IConfiguration configuration)
{
    using var scope = services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AgropecuariaContext>();

    await context.Database.EnsureCreatedAsync();

    if (context.Usuarios.Any(u => u.Rol == "Administrador"))
        return;

    var adminSection = configuration.GetSection("AdminUser");

    var admin = new Usuario
    {
        Nombre = adminSection["Nombre"] ?? "Admin",
        Apellido = adminSection["Apellido"] ?? "Principal",
        Dni = adminSection["Dni"] ?? "00000000",
        Direccion = adminSection["Direccion"] ?? "Sin dirección",
        Email = adminSection["Email"] ?? "admin@admin.com",
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminSection["Password"] ?? "Admin123"),
        Rol = "Administrador",
        FechaRegistro = DateTime.Now
    };

    context.Usuarios.Add(admin);
    await context.SaveChangesAsync();
}

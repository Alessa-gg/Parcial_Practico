using EquiposReportes.Data;
using EquiposReportes.Filters;
using EquiposReportes.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(o => o.Filters.Add<SoloAdminFilter>());

// Cadena de conexión del usuario "reportes" (db_datareader): viene de User Secrets, no del código.
var connection = builder.Configuration.GetConnectionString("ReportesDb")
    ?? throw new InvalidOperationException(
        "Falta ConnectionStrings:ReportesDb. Defínala en User Secrets (dotnet user-secrets set).");
builder.Services.AddDbContext<EquiposDbContext>(o => o.UseSqlServer(connection));

builder.Services.AddScoped<ReporteService>();

builder.Services.AddSession(o =>
{
    o.IdleTimeout = TimeSpan.FromMinutes(30);
    o.Cookie.HttpOnly = true;
    o.Cookie.IsEssential = true;
});

var apiBaseUrl = builder.Configuration["ApiBaseUrl"]
    ?? throw new InvalidOperationException("Falta la configuración 'ApiBaseUrl'.");
if (!apiBaseUrl.EndsWith('/')) apiBaseUrl += "/";
builder.Services.AddHttpClient<AuthService>(c => c.BaseAddress = new Uri(apiBaseUrl));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Reportes}/{action=Inventario}/{id?}");

app.Run();

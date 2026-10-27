using EquiposMVC.Filters;
using EquiposMVC.Handlers;
using EquiposMVC.Services;
using Microsoft.AspNetCore.Mvc.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(o => o.Filters.Add<ApiExceptionFilter>());

builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(o =>
{
    o.IdleTimeout = TimeSpan.FromMinutes(30);
    o.Cookie.HttpOnly = true;
    o.Cookie.IsEssential = true;
});

builder.Services.AddTransient<AuthTokenHandler>();

var apiBaseUrl = builder.Configuration["ApiBaseUrl"]
    ?? throw new InvalidOperationException("Falta la configuración 'ApiBaseUrl'.");
if (!apiBaseUrl.EndsWith('/')) apiBaseUrl += "/";
var apiUri = new Uri(apiBaseUrl);

builder.Services.AddHttpClient<EquipoService>(c => c.BaseAddress = apiUri)
    .AddHttpMessageHandler<AuthTokenHandler>();
builder.Services.AddHttpClient<CategoriaService>(c => c.BaseAddress = apiUri)
    .AddHttpMessageHandler<AuthTokenHandler>();
// Susan registra aquí sus servicios de la misma forma.

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Equipos}/{action=Index}/{id?}");

app.Run();
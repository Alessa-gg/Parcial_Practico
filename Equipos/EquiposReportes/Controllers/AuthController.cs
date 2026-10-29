using EquiposReportes.Filters;
using EquiposReportes.Services;
using EquiposReportes.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EquiposReportes.Controllers;

public class AuthController : Controller
{
    private readonly AuthService _auth;

    public AuthController(AuthService auth)
    {
        _auth = auth;
    }

    [SinSesion]
    public IActionResult Login() => View(new LoginViewModel());

    [HttpPost, ValidateAntiForgeryToken, SinSesion]
    public async Task<IActionResult> Login(LoginViewModel m)
    {
        if (!ModelState.IsValid) return View(m);

        var (data, error) = await _auth.LoginAsync(m);
        if (data is null)
        {
            ModelState.AddModelError(string.Empty, error!);
            return View(m);
        }

        // Los reportes son solo para el rol Admin.
        if (data.Rol != "Admin")
        {
            ModelState.AddModelError(string.Empty, "Solo los administradores pueden acceder a los reportes.");
            return View(m);
        }

        HttpContext.Session.SetString("JWToken", data.Token);
        HttpContext.Session.SetString("Rol", data.Rol);
        HttpContext.Session.SetString("Username", data.Username);

        return RedirectToAction("Inventario", "Reportes");
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction(nameof(Login));
    }
}

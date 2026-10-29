using EquiposMVC.Filters;
using EquiposMVC.Models;
using EquiposMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace EquiposMVC.Controllers;

public class AuthController : Controller
{
    private readonly AuthService _auth;

    public AuthController(AuthService auth)
    {
        _auth = auth;
    }

    [SinSesion]
    public IActionResult Login(bool expirada = false)
    {
        if (expirada)
            ViewBag.Aviso = "Tu sesión expiró. Inicia sesión de nuevo.";
        return View(new LoginViewModel());
    }

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

        HttpContext.Session.SetString("JWToken", data.Token);
        HttpContext.Session.SetString("Rol", data.Rol);
        HttpContext.Session.SetString("Username", data.Username);
        HttpContext.Session.SetInt32("UsuarioId", data.UsuarioId);

        return RedirectToAction("Index", "Equipos");
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction(nameof(Login));
    }

    [SoloRoles("Admin")]
    public IActionResult Register() => View(new RegistrarUsuarioViewModel());

    [HttpPost, ValidateAntiForgeryToken, SoloRoles("Admin")]
    public async Task<IActionResult> Register(RegistrarUsuarioViewModel m)
    {
        if (ModelState.IsValid)
        {
            var (ok, error) = await _auth.RegistrarAsync(m);
            if (ok)
            {
                TempData["Ok"] = $"Usuario '{m.Username}' registrado.";
                return RedirectToAction(nameof(Register));
            }
            ModelState.AddModelError(string.Empty, error!);   // 409 sin perder lo escrito
        }
        m.Password = string.Empty;
        return View(m);
    }
}

using EquiposMVC.Filters;
using EquiposMVC.Models;
using EquiposMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EquiposMVC.Controllers;

public class PrestamosController : Controller
{
    private readonly PrestamoService _prestamos;
    private readonly EquipoService _equipos;

    public PrestamosController(PrestamoService prestamos, EquipoService equipos)
    {
        _prestamos = prestamos;
        _equipos = equipos;
    }

    // Admin y Docente ven todos; el Estudiante ve solo los suyos (lo decide la API con el token).
    public async Task<IActionResult> Index(bool soloVencidos = false)
    {
        return View(new PrestamosIndexViewModel
        {
            Items = await _prestamos.ListarAsync(soloVencidos),
            SoloVencidos = soloVencidos
        });
    }

    private async Task CargarListasAsync(int? equipoId = null, int? usuarioId = null)
    {
        var equipos = await _equipos.ListarAsync(null, null, "Disponible");
        ViewBag.Equipos = new SelectList(
            equipos.Select(e => new { e.Id, Texto = $"{e.CodigoInventario} - {e.Nombre}" }),
            "Id", "Texto", equipoId);

        var usuarios = await _prestamos.ListarUsuariosAsync();
        ViewBag.Usuarios = new SelectList(
            usuarios.Select(u => new { u.Id, Texto = $"{u.Username} ({u.Rol})" }),
            "Id", "Texto", usuarioId);
    }

    [SoloRoles("Admin", "Docente")]
    public async Task<IActionResult> Create()
    {
        await CargarListasAsync();
        return View(new PrestamoCreateViewModel { FechaVencimiento = DateTime.Today.AddDays(3) });
    }

    [HttpPost, ValidateAntiForgeryToken, SoloRoles("Admin", "Docente")]
    public async Task<IActionResult> Create(PrestamoCreateViewModel m)
    {
        if (ModelState.IsValid)
        {
            var (ok, error) = await _prestamos.RegistrarAsync(m);
            if (ok)
            {
                TempData["Ok"] = "Préstamo registrado.";
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError(string.Empty, error!);   // 409 / 400 sin perder lo escrito
        }

        await CargarListasAsync(m.EquipoId, m.UsuarioId);
        return View(m);
    }

    [HttpPost, ValidateAntiForgeryToken, SoloRoles("Admin", "Docente")]
    public async Task<IActionResult> Devolver(int id)
    {
        var (ok, error) = await _prestamos.DevolverAsync(id);
        if (ok) TempData["Ok"] = "Devolución registrada.";
        else TempData["Error"] = error;
        return RedirectToAction(nameof(Index));
    }
}

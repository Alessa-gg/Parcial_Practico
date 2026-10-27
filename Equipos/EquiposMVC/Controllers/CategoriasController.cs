using EquiposMVC.Models;
using EquiposMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace EquiposMVC.Controllers;

public class CategoriasController : Controller
{
    private readonly CategoriaService _svc;

    public CategoriasController(CategoriaService svc)
    {
        _svc = svc;
    }

    public async Task<IActionResult> Index() => View(await _svc.ListarAsync());

    public IActionResult Create() => View(new CategoriaViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoriaViewModel m)
    {
        if (ModelState.IsValid)
        {
            var (ok, error) = await _svc.CrearAsync(m);
            if (ok) return RedirectToAction(nameof(Index));
            ModelState.AddModelError(string.Empty, error!);
        }
        return View(m);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var c = await _svc.ObtenerAsync(id);
        return c is null ? NotFound() : View(c);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(CategoriaViewModel m)
    {
        if (ModelState.IsValid)
        {
            var (ok, error) = await _svc.ActualizarAsync(m);
            if (ok) return RedirectToAction(nameof(Index));
            ModelState.AddModelError(string.Empty, error!);
        }
        return View(m);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var c = await _svc.ObtenerAsync(id);
        return c is null ? NotFound() : View(c);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var (ok, error) = await _svc.EliminarAsync(id);
        if (!ok)
        {
            TempData["Error"] = error;
            return RedirectToAction(nameof(Delete), new { id });
        }
        return RedirectToAction(nameof(Index));
    }
}
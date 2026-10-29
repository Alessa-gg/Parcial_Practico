using EquiposMVC.Filters;
using EquiposMVC.Models;
using EquiposMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EquiposMVC.Controllers;

public class EquiposController : Controller
{
    private const int TamPagina = 10;

    private readonly EquipoService _equipos;
    private readonly CategoriaService _categorias;

    public EquiposController(EquipoService equipos, CategoriaService categorias)
    {
        _equipos = equipos;
        _categorias = categorias;
    }

    private async Task CargarCategoriasAsync(int? seleccionada = null)
    {
        var cats = await _categorias.ListarAsync();
        ViewBag.Categorias = new SelectList(cats, "Id", "Nombre", seleccionada);
    }

    public async Task<IActionResult> Index(string? texto, int? categoriaId, string? estado, int pagina = 1)
    {
        // El Estudiante solo consulta equipos disponibles.
        if (HttpContext.Session.GetString("Rol") == "Estudiante") estado = "Disponible";

        var todos = await _equipos.ListarAsync(texto, categoriaId, estado);

        var totalPaginas = Math.Max(1, (int)Math.Ceiling(todos.Count / (double)TamPagina));
        pagina = Math.Clamp(pagina, 1, totalPaginas);

        await CargarCategoriasAsync(categoriaId);

        return View(new EquiposIndexViewModel
        {
            Items = todos.Skip((pagina - 1) * TamPagina).Take(TamPagina).ToList(),
            Texto = texto,
            CategoriaId = categoriaId,
            Estado = estado,
            Pagina = pagina,
            TotalPaginas = totalPaginas
        });
    }

    [SoloRoles("Admin")]
    public async Task<IActionResult> Create()
    {
        await CargarCategoriasAsync();
        return View(new EquipoFormViewModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    [SoloRoles("Admin")]
    public async Task<IActionResult> Create(EquipoFormViewModel m)
    {
        if (ModelState.IsValid)
        {
            var (ok, error) = await _equipos.CrearAsync(m);
            if (ok) return RedirectToAction(nameof(Index));
            ModelState.AddModelError(string.Empty, error!);
        }

        await CargarCategoriasAsync(m.CategoriaId);
        return View(m);
    }

    [SoloRoles("Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var e = await _equipos.ObtenerAsync(id);
        if (e is null) return NotFound();

        await CargarCategoriasAsync(e.CategoriaId);
        return View(new EquipoFormViewModel
        {
            Id = e.Id,
            CodigoInventario = e.CodigoInventario,
            Nombre = e.Nombre,
            Descripcion = e.Descripcion,
            CategoriaId = e.CategoriaId,
            Estado = e.Estado,
            RowVersion = e.RowVersion
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [SoloRoles("Admin")]
    public async Task<IActionResult> Edit(EquipoFormViewModel m)
    {
        if (ModelState.IsValid)
        {
            var (ok, error) = await _equipos.ActualizarAsync(m);
            if (ok) return RedirectToAction(nameof(Index));
            ModelState.AddModelError(string.Empty, error!);   // 409 sin perder lo escrito
        }

        await CargarCategoriasAsync(m.CategoriaId);
        return View(m);
    }

    [SoloRoles("Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var e = await _equipos.ObtenerAsync(id);
        return e is null ? NotFound() : View(e);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    [SoloRoles("Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var (ok, error) = await _equipos.EliminarAsync(id);
        if (!ok)
        {
            TempData["Error"] = error;
            return RedirectToAction(nameof(Delete), new { id });
        }
        return RedirectToAction(nameof(Index));
    }
}
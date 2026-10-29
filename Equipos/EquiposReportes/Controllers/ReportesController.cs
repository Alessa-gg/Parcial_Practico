using EquiposReportes.Services;
using EquiposReportes.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EquiposReportes.Controllers;

public class ReportesController : Controller
{
    private readonly ReporteService _reportes;

    public ReportesController(ReporteService reportes)
    {
        _reportes = reportes;
    }

    public async Task<IActionResult> Inventario(FiltroReporte filtro)
    {
        return View(new ReporteViewModel<InventarioCategoriaVM>
        {
            Filtro = filtro,
            Items = await _reportes.InventarioPorCategoriaAsync(filtro),
            Categorias = await _reportes.CategoriasAsync()
        });
    }

    public async Task<IActionResult> Vencidos(FiltroReporte filtro)
    {
        return View(new ReporteViewModel<PrestamoVencidoVM>
        {
            Filtro = filtro,
            Items = await _reportes.PrestamosVencidosAsync(filtro),
            Categorias = await _reportes.CategoriasAsync()
        });
    }

    public async Task<IActionResult> Top(FiltroReporte filtro)
    {
        return View(new ReporteViewModel<TopEquipoVM>
        {
            Filtro = filtro,
            Items = await _reportes.TopEquiposAsync(filtro),
            Categorias = await _reportes.CategoriasAsync()
        });
    }

    public async Task<IActionResult> Historial(FiltroReporte filtro)
    {
        return View(new ReporteViewModel<HistorialUsuarioVM>
        {
            Filtro = filtro,
            Items = await _reportes.HistorialPorUsuarioAsync(filtro),
            Categorias = await _reportes.CategoriasAsync()
        });
    }
}

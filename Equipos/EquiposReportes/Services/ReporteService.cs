using EquiposReportes.Data;
using EquiposReportes.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EquiposReportes.Services;

// Solo lectura: todas las consultas usan AsNoTracking() y proyectan a ViewModels.
public class ReporteService
{
    private readonly EquiposDbContext _db;

    public ReporteService(EquiposDbContext db)
    {
        _db = db;
    }

    public async Task<List<SelectListItem>> CategoriasAsync() =>
        await _db.Categorias.AsNoTracking()
            .OrderBy(c => c.Nombre)
            .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Nombre })
            .ToListAsync();

    // 1. Cantidad de equipos por categoría y por estado (foto actual del inventario).
    public async Task<List<InventarioCategoriaVM>> InventarioPorCategoriaAsync(FiltroReporte f)
    {
        return await _db.Categorias.AsNoTracking()
            .Where(c => !f.CategoriaId.HasValue || c.Id == f.CategoriaId)
            .OrderBy(c => c.Nombre)
            .Select(c => new InventarioCategoriaVM
            {
                Categoria = c.Nombre,
                Disponibles = c.Equipos.Count(e => e.Estado == "Disponible"),
                Prestados = c.Equipos.Count(e => e.Estado == "Prestado"),
                Mantenimiento = c.Equipos.Count(e => e.Estado == "Mantenimiento"),
                Total = c.Equipos.Count()
            })
            .ToListAsync();
    }

    // 2. FechaVencimiento menor a hoy y FechaDevolucion nula, con días de atraso.
    public async Task<List<PrestamoVencidoVM>> PrestamosVencidosAsync(FiltroReporte f)
    {
        var hoy = DateTime.Today;
        var desde = f.Desde?.Date;
        var hastaExcl = f.Hasta?.Date.AddDays(1);

        var q = _db.Prestamos.AsNoTracking()
            .Where(p => p.FechaDevolucion == null && p.FechaVencimiento < hoy);

        if (desde.HasValue) q = q.Where(p => p.FechaPrestamo >= desde.Value);
        if (hastaExcl.HasValue) q = q.Where(p => p.FechaPrestamo < hastaExcl.Value);
        if (f.CategoriaId.HasValue) q = q.Where(p => p.Equipo.CategoriaId == f.CategoriaId.Value);

        return await q
            .OrderBy(p => p.FechaVencimiento)
            .Select(p => new PrestamoVencidoVM
            {
                Id = p.Id,
                Username = p.Usuario.Username,
                Codigo = p.Equipo.CodigoInventario,
                Equipo = p.Equipo.Nombre,
                Categoria = p.Equipo.Categoria.Nombre,
                FechaPrestamo = p.FechaPrestamo,
                FechaVencimiento = p.FechaVencimiento,
                DiasAtraso = EF.Functions.DateDiffDay(p.FechaVencimiento, hoy)
            })
            .ToListAsync();
    }

    // 3. Los 5 equipos más prestados en un rango de fechas.
    public async Task<List<TopEquipoVM>> TopEquiposAsync(FiltroReporte f)
    {
        var desde = f.Desde?.Date;
        var hastaExcl = f.Hasta?.Date.AddDays(1);

        var q = _db.Prestamos.AsNoTracking().AsQueryable();
        if (desde.HasValue) q = q.Where(p => p.FechaPrestamo >= desde.Value);
        if (hastaExcl.HasValue) q = q.Where(p => p.FechaPrestamo < hastaExcl.Value);
        if (f.CategoriaId.HasValue) q = q.Where(p => p.Equipo.CategoriaId == f.CategoriaId.Value);

        return await q
            .GroupBy(p => new
            {
                p.EquipoId,
                p.Equipo.CodigoInventario,
                p.Equipo.Nombre,
                Categoria = p.Equipo.Categoria.Nombre
            })
            .Select(g => new TopEquipoVM
            {
                Codigo = g.Key.CodigoInventario,
                Equipo = g.Key.Nombre,
                Categoria = g.Key.Categoria,
                TotalPrestamos = g.Count()
            })
            .OrderByDescending(x => x.TotalPrestamos)
            .ThenBy(x => x.Equipo)
            .Take(5)
            .ToListAsync();
    }

    // 4. Búsqueda por Username con todos sus préstamos.
    public async Task<List<HistorialUsuarioVM>> HistorialPorUsuarioAsync(FiltroReporte f)
    {
        if (string.IsNullOrWhiteSpace(f.Username)) return new();

        var texto = f.Username.Trim();
        var desde = f.Desde?.Date;
        var hastaExcl = f.Hasta?.Date.AddDays(1);

        var q = _db.Prestamos.AsNoTracking()
            .Where(p => p.Usuario.Username.Contains(texto));

        if (desde.HasValue) q = q.Where(p => p.FechaPrestamo >= desde.Value);
        if (hastaExcl.HasValue) q = q.Where(p => p.FechaPrestamo < hastaExcl.Value);
        if (f.CategoriaId.HasValue) q = q.Where(p => p.Equipo.CategoriaId == f.CategoriaId.Value);

        return await q
            .OrderByDescending(p => p.FechaPrestamo)
            .Select(p => new HistorialUsuarioVM
            {
                Username = p.Usuario.Username,
                Codigo = p.Equipo.CodigoInventario,
                Equipo = p.Equipo.Nombre,
                Categoria = p.Equipo.Categoria.Nombre,
                FechaPrestamo = p.FechaPrestamo,
                FechaVencimiento = p.FechaVencimiento,
                FechaDevolucion = p.FechaDevolucion
            })
            .ToListAsync();
    }
}

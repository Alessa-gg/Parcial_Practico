using AutoMapper;
using EquiposAPI.Data;
using EquiposAPI.Dtos;
using EquiposAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EquiposAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize]
public class EquiposController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public EquiposController(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<EquipoListDto>>> Get(string? texto, int? categoriaId, EstadoEquipo? estado)
    {
        var q = _db.Equipos.AsNoTracking().Include(e => e.Categoria).AsQueryable();

        if (!string.IsNullOrWhiteSpace(texto))
            q = q.Where(e => e.Nombre.Contains(texto) || e.CodigoInventario.Contains(texto));
        if (categoriaId.HasValue)
            q = q.Where(e => e.CategoriaId == categoriaId.Value);
        if (estado.HasValue)
            q = q.Where(e => e.Estado == estado.Value);

        var lista = await q.OrderBy(e => e.Nombre).ToListAsync();
        return _mapper.Map<List<EquipoListDto>>(lista);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EquipoDetalleDto>> GetById(int id)
    {
        var equipo = await _db.Equipos.AsNoTracking()
            .Include(e => e.Categoria)
            .FirstOrDefaultAsync(e => e.Id == id);
        return equipo is null ? NotFound() : _mapper.Map<EquipoDetalleDto>(equipo);
    }

    [HttpPost]
    // [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(EquipoCreateDto dto)
    {
        if (await _db.Equipos.AnyAsync(e => e.CodigoInventario == dto.CodigoInventario))
            return Conflict("Ya existe un equipo con ese código de inventario.");
        if (!await _db.Categorias.AnyAsync(c => c.Id == dto.CategoriaId))
            return BadRequest("La categoría no existe.");

        var equipo = _mapper.Map<Equipo>(dto);
        equipo.Estado = EstadoEquipo.Disponible;
        _db.Equipos.Add(equipo);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = equipo.Id }, new { equipo.Id });
    }

    [HttpPut("{id}")]
    // [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, EquipoUpdateDto dto)
    {
        var equipo = await _db.Equipos.FindAsync(id);
        if (equipo is null) return NotFound();

        if (await _db.Equipos.AnyAsync(e => e.CodigoInventario == dto.CodigoInventario && e.Id != id))
            return Conflict("Ya existe otro equipo con ese código de inventario.");
        if (!await _db.Categorias.AnyAsync(c => c.Id == dto.CategoriaId))
            return BadRequest("La categoría no existe.");

      
        equipo.CodigoInventario = dto.CodigoInventario;
        equipo.Nombre = dto.Nombre;
        equipo.Descripcion = dto.Descripcion;
        equipo.CategoriaId = dto.CategoriaId;
        equipo.Estado = dto.Estado;

        _db.Entry(equipo).Property(e => e.RowVersion).OriginalValue = dto.RowVersion;

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict("Otro usuario modificó este equipo. Recarga la página y vuelve a intentar.");
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    // [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var equipo = await _db.Equipos.FindAsync(id);
        if (equipo is null) return NotFound();

        if (await _db.Prestamos.AnyAsync(p => p.EquipoId == id))
            return Conflict("No se puede eliminar: el equipo tiene préstamos históricos.");

        _db.Equipos.Remove(equipo);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

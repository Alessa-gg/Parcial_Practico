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
[Authorize]
public class CategoriasController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public CategoriasController(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<CategoriaDto>>> Get()
    {
        var lista = await _db.Categorias.AsNoTracking().OrderBy(c => c.Nombre).ToListAsync();
        return _mapper.Map<List<CategoriaDto>>(lista);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoriaDto>> GetById(int id)
    {
        var cat = await _db.Categorias.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        return cat is null ? NotFound() : _mapper.Map<CategoriaDto>(cat);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CategoriaSaveDto dto)
    {
        if (await _db.Categorias.AnyAsync(c => c.Nombre == dto.Nombre))
            return Conflict("Ya existe una categoría con ese nombre.");

        var cat = _mapper.Map<Categoria>(dto);
        _db.Categorias.Add(cat);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = cat.Id }, _mapper.Map<CategoriaDto>(cat));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, CategoriaSaveDto dto)
    {
        var cat = await _db.Categorias.FindAsync(id);
        if (cat is null) return NotFound();

        if (await _db.Categorias.AnyAsync(c => c.Nombre == dto.Nombre && c.Id != id))
            return Conflict("Ya existe otra categoría con ese nombre.");

        cat.Nombre = dto.Nombre;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var cat = await _db.Categorias.FindAsync(id);
        if (cat is null) return NotFound();

        if (await _db.Equipos.AnyAsync(e => e.CategoriaId == id))
            return Conflict("No se puede eliminar: la categoría tiene equipos asociados.");

        _db.Categorias.Remove(cat);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
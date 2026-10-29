using System.Security.Claims;
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
public class PrestamosController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public PrestamosController(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    // Admin y Docente ven todos los préstamos; el Estudiante solo los propios.
    // El Id del usuario se toma SIEMPRE del token, nunca de un parámetro.
    [HttpGet]
    public async Task<ActionResult<List<PrestamoListDto>>> Get(bool soloVencidos = false)
    {
        var q = _db.Prestamos.AsNoTracking()
            .Include(p => p.Equipo)
            .Include(p => p.Usuario)
            .AsQueryable();

        if (!User.IsInRole("Admin") && !User.IsInRole("Docente"))
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var usuarioId))
                return Unauthorized();
            q = q.Where(p => p.UsuarioId == usuarioId);
        }

        if (soloVencidos)
        {
            var hoy = DateTime.Today;
            q = q.Where(p => p.FechaDevolucion == null && p.FechaVencimiento < hoy);
        }

        var lista = await q.OrderByDescending(p => p.FechaPrestamo).ToListAsync();
        return _mapper.Map<List<PrestamoListDto>>(lista);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Docente")]
    public async Task<IActionResult> Registrar(PrestamoCreateDto dto)
    {
        var hoy = DateTime.Today;
        if (dto.FechaVencimiento.Date < hoy || dto.FechaVencimiento.Date > hoy.AddDays(7))
            return BadRequest("La fecha de vencimiento debe estar entre hoy y 7 días desde hoy.");

        var equipo = await _db.Equipos.FindAsync(dto.EquipoId);
        if (equipo is null) return NotFound("El equipo no existe.");

        if (equipo.Estado != EstadoEquipo.Disponible)
            return Conflict($"El equipo no está disponible (estado actual: {equipo.Estado}).");

        if (!await _db.Usuarios.AnyAsync(u => u.Id == dto.UsuarioId))
            return NotFound("El usuario no existe.");

        var prestamo = new Prestamo
        {
            EquipoId = dto.EquipoId,
            UsuarioId = dto.UsuarioId,
            FechaPrestamo = DateTime.Now,
            FechaVencimiento = dto.FechaVencimiento.Date,
            Observaciones = dto.Observaciones
        };

        equipo.Estado = EstadoEquipo.Prestado;
        _db.Prestamos.Add(prestamo);

        try
        {
            // Un solo SaveChanges = una sola transacción
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict("Otro usuario modificó el equipo al mismo tiempo. Intenta de nuevo.");
        }

        return Ok(new { prestamo.Id });
    }

    [HttpPut("{id}/devolucion")]
    [Authorize(Roles = "Admin,Docente")]
    public async Task<IActionResult> Devolver(int id)
    {
        var prestamo = await _db.Prestamos.Include(p => p.Equipo).FirstOrDefaultAsync(p => p.Id == id);
        if (prestamo is null) return NotFound();

        if (prestamo.FechaDevolucion != null)
            return Conflict("El préstamo ya fue devuelto.");

        prestamo.FechaDevolucion = DateTime.Now;
        prestamo.Equipo!.Estado = EstadoEquipo.Disponible;

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict("El equipo fue modificado por otro usuario. Intenta de nuevo.");
        }

        return NoContent();
    }
}
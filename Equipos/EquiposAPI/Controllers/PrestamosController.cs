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
public class PrestamosController : ControllerBase
{
    private readonly AppDbContext _db;

    public PrestamosController(AppDbContext db)
    {
        _db = db;
    }

    // El GET de préstamos (filtrado por rol con el Id del token) lo implementa la Persona B.

    [HttpPost]
    // [Authorize(Roles = "Admin,Docente")]
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
    // [Authorize(Roles = "Admin,Docente")]
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
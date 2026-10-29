using AutoMapper;
using EquiposAPI.Data;
using EquiposAPI.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EquiposAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Docente")]
public class UsuariosController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public UsuariosController(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    // Lo usa el cliente MVC para elegir a quién se le presta el equipo. Nunca expone el hash.
    [HttpGet]
    public async Task<ActionResult<List<UsuarioListDto>>> Get()
    {
        var lista = await _db.Usuarios.AsNoTracking().OrderBy(u => u.Username).ToListAsync();
        return _mapper.Map<List<UsuarioListDto>>(lista);
    }
}

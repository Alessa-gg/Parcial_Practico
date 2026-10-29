using AutoMapper;
using EquiposAPI.Data;
using EquiposAPI.Dtos;
using EquiposAPI.Models;
using EquiposAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EquiposAPI.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private static readonly string[] RolesValidos = { "Admin", "Docente", "Estudiante" };

    private readonly AppDbContext _db;
    private readonly ITokenService _tokens;
    private readonly IMapper _mapper;

    public AuthController(AppDbContext db, ITokenService tokens, IMapper mapper)
    {
        _db = db;
        _tokens = tokens;
        _mapper = mapper;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginDto dto)
    {
        var usuario = await _db.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Username == dto.Username);

        // Mismo mensaje para usuario inexistente y contraseña incorrecta.
        if (usuario is null || !BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash))
            return Unauthorized("Usuario o contraseña incorrectos.");

        var (token, expira) = _tokens.Crear(usuario);
        return new LoginResponseDto
        {
            Token = token,
            Expira = expira,
            UsuarioId = usuario.Id,
            Username = usuario.Username,
            Rol = usuario.Rol
        };
    }

    [HttpPost("register")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        if (!RolesValidos.Contains(dto.Rol))
            return BadRequest("El rol debe ser Admin, Docente o Estudiante.");

        if (await _db.Usuarios.AnyAsync(u => u.Username == dto.Username))
            return Conflict("Ya existe un usuario con ese nombre de usuario.");

        var usuario = new Usuario
        {
            Username = dto.Username,
            Email = dto.Email,
            Rol = dto.Rol,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        _db.Usuarios.Add(usuario);
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            // Dos registros simultáneos con el mismo Username: lo frena el índice único.
            return Conflict("Ya existe un usuario con ese nombre de usuario.");
        }

        return StatusCode(StatusCodes.Status201Created, _mapper.Map<UsuarioListDto>(usuario));
    }
}

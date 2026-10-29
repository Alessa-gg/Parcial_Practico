using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EquiposAPI.Models;
using Microsoft.IdentityModel.Tokens;

namespace EquiposAPI.Services;

public interface ITokenService
{
    (string Token, DateTime Expira) Crear(Usuario usuario);
}

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config)
    {
        _config = config;
    }

    public (string Token, DateTime Expira) Crear(Usuario usuario)
    {
        var key = _config["Jwt:Key"]
            ?? throw new InvalidOperationException("Falta Jwt:Key en User Secrets o variables de entorno.");
        var minutos = int.TryParse(_config["Jwt:ExpireMinutes"], out var m) ? m : 60;
        var expira = DateTime.UtcNow.AddMinutes(minutos);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.Username),
            new Claim(ClaimTypes.Role, usuario.Rol)
        };

        var creds = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expira,
            signingCredentials: creds);

        return (new JwtSecurityTokenHandler().WriteToken(jwt), expira);
    }
}

using EquiposAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EquiposAPI.Data;

public static class DbSeeder
{
    // Si la tabla Usuarios está vacía crea un Admin inicial.
    // La contraseña viene de configuración (User Secrets / variables de entorno), nunca del código.
    public static async Task SembrarAdminAsync(IServiceProvider sp)
    {
        var db = sp.GetRequiredService<AppDbContext>();
        var config = sp.GetRequiredService<IConfiguration>();
        var log = sp.GetRequiredService<ILoggerFactory>().CreateLogger("DbSeeder");

        if (await db.Usuarios.AnyAsync()) return;

        var password = config["AdminSeed:Password"];
        if (string.IsNullOrWhiteSpace(password))
        {
            log.LogWarning("No hay usuarios y falta AdminSeed:Password en la configuración; no se creó el Admin inicial.");
            return;
        }

        db.Usuarios.Add(new Usuario
        {
            Username = config["AdminSeed:Username"] ?? "admin",
            Email = config["AdminSeed:Email"] ?? "admin@equipos.local",
            Rol = "Admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
        });
        await db.SaveChangesAsync();
        log.LogInformation("Admin inicial creado.");
    }
}

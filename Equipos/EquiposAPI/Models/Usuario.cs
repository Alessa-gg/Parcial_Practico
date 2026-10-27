namespace EquiposAPI.Models;

public class Usuario
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Rol { get; set; } = "Estudiante";
    public ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();
}
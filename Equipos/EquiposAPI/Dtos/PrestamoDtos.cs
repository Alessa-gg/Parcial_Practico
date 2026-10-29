using System.ComponentModel.DataAnnotations;

namespace EquiposAPI.Dtos;

public class PrestamoCreateDto
{
    [Required]
    public int EquipoId { get; set; }

    [Required]
    public int UsuarioId { get; set; }

    [Required]
    public DateTime FechaVencimiento { get; set; }

    public string? Observaciones { get; set; }
}

public class PrestamoListDto
{
    public int Id { get; set; }
    public int EquipoId { get; set; }
    public string EquipoCodigo { get; set; } = string.Empty;
    public string EquipoNombre { get; set; } = string.Empty;
    public int UsuarioId { get; set; }
    public string Username { get; set; } = string.Empty;
    public DateTime FechaPrestamo { get; set; }
    public DateTime FechaVencimiento { get; set; }
    public DateTime? FechaDevolucion { get; set; }
    public string? Observaciones { get; set; }
    public bool Vencido { get; set; }
}

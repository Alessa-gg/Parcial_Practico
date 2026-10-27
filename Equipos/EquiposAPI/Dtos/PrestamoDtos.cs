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
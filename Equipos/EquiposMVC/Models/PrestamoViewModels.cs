using System.ComponentModel.DataAnnotations;

namespace EquiposMVC.Models;

public class PrestamoViewModel
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

public class PrestamosIndexViewModel
{
    public List<PrestamoViewModel> Items { get; set; } = new();
    public bool SoloVencidos { get; set; }
}

public class PrestamoCreateViewModel
{
    [Required(ErrorMessage = "Seleccione un equipo.")]
    [Display(Name = "Equipo")]
    public int? EquipoId { get; set; }

    [Required(ErrorMessage = "Seleccione el usuario que recibe el equipo.")]
    [Display(Name = "Usuario")]
    public int? UsuarioId { get; set; }

    [Required(ErrorMessage = "La fecha de vencimiento es obligatoria.")]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha de vencimiento")]
    public DateTime? FechaVencimiento { get; set; }

    [StringLength(500)]
    public string? Observaciones { get; set; }
}

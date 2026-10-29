using Microsoft.AspNetCore.Mvc.Rendering;

namespace EquiposReportes.ViewModels;

public class FiltroReporte
{
    public DateTime? Desde { get; set; }
    public DateTime? Hasta { get; set; }
    public int? CategoriaId { get; set; }
    public string? Username { get; set; }   // solo para el historial por usuario
}

public class ReporteViewModel<T>
{
    public FiltroReporte Filtro { get; set; } = new();
    public List<T> Items { get; set; } = new();
    public List<SelectListItem> Categorias { get; set; } = new();
}

public class InventarioCategoriaVM
{
    public string Categoria { get; set; } = string.Empty;
    public int Disponibles { get; set; }
    public int Prestados { get; set; }
    public int Mantenimiento { get; set; }
    public int Total { get; set; }
}

public class PrestamoVencidoVM
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
    public string Equipo { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public DateTime FechaPrestamo { get; set; }
    public DateTime FechaVencimiento { get; set; }
    public int DiasAtraso { get; set; }
}

public class TopEquipoVM
{
    public string Codigo { get; set; } = string.Empty;
    public string Equipo { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public int TotalPrestamos { get; set; }
}

public class HistorialUsuarioVM
{
    public string Username { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
    public string Equipo { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public DateTime FechaPrestamo { get; set; }
    public DateTime FechaVencimiento { get; set; }
    public DateTime? FechaDevolucion { get; set; }

    public string Estado =>
        FechaDevolucion != null ? "Devuelto"
        : FechaVencimiento < DateTime.Today ? "Vencido"
        : "Activo";
}

public class LoginViewModel
{
    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "El usuario es obligatorio.")]
    [System.ComponentModel.DataAnnotations.Display(Name = "Usuario")]
    public string Username { get; set; } = string.Empty;

    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "La contraseña es obligatoria.")]
    [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
    [System.ComponentModel.DataAnnotations.Display(Name = "Contraseña")]
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expira { get; set; }
    public int UsuarioId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
}

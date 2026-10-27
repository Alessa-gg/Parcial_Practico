using System.ComponentModel.DataAnnotations;

namespace EquiposMVC.Models;

public class EquipoViewModel
{
    public int Id { get; set; }
    public string CodigoInventario { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int CategoriaId { get; set; }
    public string? RowVersion { get; set; }
}

public class EquipoFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El código es obligatorio.")]
    [StringLength(50)]
    [Display(Name = "Código de inventario")]
    public string CodigoInventario { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(200)]
    public string Nombre { get; set; } = string.Empty;

    [Display(Name = "Descripción")]
    public string? Descripcion { get; set; }

    [Required(ErrorMessage = "Seleccione una categoría.")]
    [Display(Name = "Categoría")]
    public int? CategoriaId { get; set; }

    public string? Estado { get; set; }

    public string? RowVersion { get; set; }
}

public class EquiposIndexViewModel
{
    public List<EquipoViewModel> Items { get; set; } = new();
    public string? Texto { get; set; }
    public int? CategoriaId { get; set; }
    public string? Estado { get; set; }
    public int Pagina { get; set; } = 1;
    public int TotalPaginas { get; set; } = 1;
}

public class CategoriaViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;
}
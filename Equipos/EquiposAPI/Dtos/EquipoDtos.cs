using System.ComponentModel.DataAnnotations;
using EquiposAPI.Models;

namespace EquiposAPI.Dtos;

public class EquipoListDto
{
    public int Id { get; set; }
    public string CodigoInventario { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
}

public class EquipoDetalleDto : EquipoListDto
{
    public string? Descripcion { get; set; }
    public int CategoriaId { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}

public class EquipoCreateDto
{
    [Required, StringLength(50)]
    public string CodigoInventario { get; set; } = string.Empty;

    [Required, StringLength(200)]
    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    [Required]
    public int CategoriaId { get; set; }
}

public class EquipoUpdateDto : EquipoCreateDto
{
    [Required]
    public EstadoEquipo Estado { get; set; }

    [Required]
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
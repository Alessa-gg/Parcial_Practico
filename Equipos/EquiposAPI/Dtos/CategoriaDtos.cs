using System.ComponentModel.DataAnnotations;

namespace EquiposAPI.Dtos;

public class CategoriaDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
}

public class CategoriaSaveDto
{
    [Required, StringLength(100)]
    public string Nombre { get; set; } = string.Empty;
}
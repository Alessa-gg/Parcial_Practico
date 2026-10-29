using AutoMapper;
using EquiposAPI.Dtos;
using EquiposAPI.Models;

namespace EquiposAPI.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Categoria, CategoriaDto>();
        CreateMap<CategoriaSaveDto, Categoria>();

        CreateMap<Equipo, EquipoListDto>()
            .ForMember(d => d.Categoria, o => o.MapFrom(s => s.Categoria != null ? s.Categoria.Nombre : string.Empty))
            .ForMember(d => d.Estado, o => o.MapFrom(s => s.Estado.ToString()));

        CreateMap<Equipo, EquipoDetalleDto>()
            .ForMember(d => d.Categoria, o => o.MapFrom(s => s.Categoria != null ? s.Categoria.Nombre : string.Empty))
            .ForMember(d => d.Estado, o => o.MapFrom(s => s.Estado.ToString()));

        CreateMap<EquipoCreateDto, Equipo>();

        CreateMap<Prestamo, PrestamoListDto>()
            .ForMember(d => d.EquipoCodigo, o => o.MapFrom(s => s.Equipo != null ? s.Equipo.CodigoInventario : string.Empty))
            .ForMember(d => d.EquipoNombre, o => o.MapFrom(s => s.Equipo != null ? s.Equipo.Nombre : string.Empty))
            .ForMember(d => d.Username, o => o.MapFrom(s => s.Usuario != null ? s.Usuario.Username : string.Empty))
            .ForMember(d => d.Vencido, o => o.MapFrom(s => s.FechaDevolucion == null && s.FechaVencimiento < DateTime.Today));

        CreateMap<Usuario, UsuarioListDto>();
    }
}
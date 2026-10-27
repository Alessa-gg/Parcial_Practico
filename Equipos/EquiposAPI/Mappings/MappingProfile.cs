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
    }
}
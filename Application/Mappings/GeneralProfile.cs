using AutoMapper;
using Domain.Entities;
using Models.Response._cochera;
using Models.Response._user;
using Models.Request._cochera;

namespace Application.Mappings
{
    public class GeneralProfile : Profile
    {
        public GeneralProfile()
        {
            CreateMap<ApplicationUser, UserResponse>();

            CreateMap<Cochera, CocheraResponse>();

            CreateMap<UpdateCocheraRequest, Cochera>()
                .ForMember(dest => dest.Nombre, opt => opt.Condition(src => src.Nombre != null))
                .ForMember(dest => dest.Direccion, opt => opt.Condition(src => src.Direccion != null))
                .ForMember(dest => dest.CapacidadTotal, opt => opt.Condition(src => src.CapacidadTotal != null))
                .ForMember(dest => dest.ImagenUrl, opt => opt.Condition(src => src.ImagenUrl != null));
        }
    }
}

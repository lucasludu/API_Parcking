using AutoMapper;
using Domain.Entities;
using Models.Response._user;

namespace Application.Mappings
{
    public class GeneralProfile : Profile
    {
        public GeneralProfile()
        {
            CreateMap<ApplicationUser, UserResponse>();
        }
    }
}

using AutoMapper;
using Identity.Controllers.Resources;
using Identity.Core.Models;

namespace Identity.Mapping
{
    public class ResourceToModelProfile : Profile
    {
        public ResourceToModelProfile()
        {
            CreateMap<UserCredentialsResource, User>()
                .ForMember(a=>a.PasswordHash,opt=>opt.MapFrom(a=>a.Password));
        }
    }
}
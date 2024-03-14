using AutoMapper;
using mumarket.DataContracts.Responses;
using mumarket.Dtos;
using mumarket.Models;

namespace mumarket.Mappers
{
    public class MuMarketProfile : Profile
    {
        public MuMarketProfile()
        {
            CreateMap<Sell, SellResponse>();
            CreateMap<UserDto, User>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.id._serialized))
                .ForMember(dst => dst.Alias, opt => opt.MapFrom(src => src.id.user))
                .ForMember(dst => dst.IsAdmin, opt => opt.MapFrom(src => src.isAdmin))
                .ForMember(dst => dst.IsOwner, opt => opt.MapFrom(src => src.isSuperAdmin))
                .ForMember(dst => dst.AddedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
        }
    }
}

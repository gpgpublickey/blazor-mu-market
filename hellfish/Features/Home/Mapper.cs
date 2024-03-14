using AutoMapper;
using hellfish.Domain;
using mumarket.DataContracts.Responses;

namespace hellfish.Features.Home
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<SellResponse, Sell>()
                .ForMember(dst => dst.Name, opt => opt.MapFrom(x => x.Author))
                .ForMember(dst => dst.Image, opt => opt.MapFrom(x => x.Img))
                .ForMember(dst => dst.TimeInMinutes, opt => opt.MapFrom(x => (DateTime.UtcNow - x.CreatedAt).TotalMinutes))
                .ForMember(dst => dst.Description, opt => opt.MapFrom(x => x.Post));
        }
    }
}

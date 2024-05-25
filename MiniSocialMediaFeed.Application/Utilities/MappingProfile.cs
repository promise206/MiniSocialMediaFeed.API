using AutoMapper;
using MiniSocialMediaFeed.Application.Dtos.RequestDto;
using MiniSocialMediaFeed.Application.Dtos.ResponseDto;
using MiniSocialMediaFeed.Domain.Entities;

namespace MiniSocialMediaFeed.Application.Utilities
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Post, PostReqDto>().ReverseMap(); // Ensure two-way mapping is configured
            CreateMap<Post, PostRespDto>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<Follow, FollowRespDto>().ReverseMap();
        }
    }
}

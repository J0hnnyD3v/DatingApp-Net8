using API.DTOs.User;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<AppUser, MemberDto>()
            .ForMember(dest => dest.Age, opt => opt.MapFrom(source => source.DateOfBirth.CalculateAge()))
            .ForMember(dest => dest.PhotoUrl, options =>
                options.MapFrom(source => source.Photos.FirstOrDefault(photo => photo.IsMain)!.Url));
        CreateMap<Photo, PhotoDto>();
    }
}

using MiniSocialMediaFeed.Application.Dtos.RequestDto;
using MiniSocialMediaFeed.Application.Dtos.ResponseDto;

namespace MiniSocialMediaFeed.Application.Interfaces
{
    public interface IAuthService
    {
        Task<ResponseDto<string>> LoginUserAsync(LoginUserReqDto loginUserDto);

        Task<ResponseDto<UserDto>> RegisterUserAsync(RegisterUserReqDto registerUserDto);
    }
}

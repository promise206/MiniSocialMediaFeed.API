using MiniSocialMediaFeed.Application.Dtos.ResponseDto;

namespace MiniSocialMediaFeed.Application.Interfaces
{
    public interface IFollowService
    {
        Task<ResponseDto<FollowRespDto>> FollowUserAsync(int followerId, int followeeId);
        Task<ResponseDto<IEnumerable<FollowRespDto>>> GetFollowersAsync(int userId);
        Task<ResponseDto<IEnumerable<FollowRespDto>>> GetFollowingAsync(int userId);
    }
}

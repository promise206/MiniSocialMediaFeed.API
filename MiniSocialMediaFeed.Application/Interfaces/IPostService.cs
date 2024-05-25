using MiniSocialMediaFeed.Application.Dtos.RequestDto;
using MiniSocialMediaFeed.Application.Dtos.ResponseDto;

namespace MiniSocialMediaFeed.Application.Interfaces
{
    public interface IPostService
    {
        Task<ResponseDto<IEnumerable<PostReqDto>>> GetFeedAsync(int userId, int pageNumber, int pageSize);
        Task<ResponseDto<PostReqDto>> CreatePostAsync(PostReqDto createPostDto);
    }
}

using AutoMapper;
using Microsoft.Extensions.Logging;
using MiniSocialMediaFeed.Application.Dtos.RequestDto;
using MiniSocialMediaFeed.Application.Dtos.ResponseDto;
using MiniSocialMediaFeed.Application.Interfaces;
using MiniSocialMediaFeed.Domain.Entities;
using System.Net;

namespace MiniSocialMediaFeed.Application.Services
{
    public class PostService : IPostService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<PostService> _logger;

        public PostService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<PostService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ResponseDto<PostReqDto>> CreatePostAsync(PostReqDto createPostDto)
        {
            _logger.LogInformation("Creating post for user ID: {UserId}", createPostDto.UserId);
            var post = _mapper.Map<Post>(createPostDto);
            post.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.Posts.AddAsync(post);
            await _unitOfWork.SaveAsync();

            var postDto = _mapper.Map<PostReqDto>(post);
            _logger.LogInformation("Post created successfully for user ID: {UserId}", createPostDto.UserId);
            return ResponseDto<PostReqDto>.Success("Post created successfully.", postDto, (int)HttpStatusCode.Created);
        }

        public async Task<ResponseDto<IEnumerable<PostReqDto>>> GetFeedAsync(int userId, int pageNumber, int pageSize)
        {
            _logger.LogInformation("Retrieving feed for user ID: {UserId}", userId);
            var follows = await _unitOfWork.Follows.GetAsync(f => f.FollowerId == userId);
            var followingIds = follows.Select(f => f.FolloweeId).ToList();

            var posts = await _unitOfWork.Posts.GetAsync(
                p => followingIds.Contains(p.UserId) || p.UserId == userId,
                orderBy: q => q.OrderByDescending(p => p.Likes),
                skip: (pageNumber - 1) * pageSize,
                take: pageSize
            );

            var postDtos = _mapper.Map<IEnumerable<PostReqDto>>(posts);
            _logger.LogInformation("Feed retrieved successfully for user ID: {UserId}", userId);
            return ResponseDto<IEnumerable<PostReqDto>>.Success("Feed retrieved successfully.", postDtos);
        }
    }
}

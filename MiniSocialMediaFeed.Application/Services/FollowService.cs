using AutoMapper;
using Microsoft.Extensions.Logging;
using MiniSocialMediaFeed.Application.Dtos.ResponseDto;
using MiniSocialMediaFeed.Application.Interfaces;
using MiniSocialMediaFeed.Domain.Entities;
using System.Net;

namespace MiniSocialMediaFeed.Application.Services
{
    public class FollowService : IFollowService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<FollowService> _logger;

        public FollowService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<FollowService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ResponseDto<FollowRespDto>> FollowUserAsync(int followerId, int followeeId)
        {
            _logger.LogInformation("User ID: {FollowerId} attempting to follow user ID: {FolloweeId}", followerId, followeeId);
            var existingFollow = await _unitOfWork.Follows.GetAsync(f => f.FollowerId == followerId && f.FolloweeId == followeeId);
            if (existingFollow.Any())
            {
                _logger.LogWarning("User ID: {FollowerId} is already following user ID: {FolloweeId}", followerId, followeeId);
                return ResponseDto<FollowRespDto>.Fail("You are already following this user.", (int)HttpStatusCode.Conflict);
            }

            var follow = new Follow
            {
                FollowerId = followerId,
                FolloweeId = followeeId
            };

            await _unitOfWork.Follows.AddAsync(follow);
            await _unitOfWork.SaveAsync();

            var followDto = _mapper.Map<FollowRespDto>(follow);
            _logger.LogInformation("User ID: {FollowerId} successfully followed user ID: {FolloweeId}", followerId, followeeId);
            return ResponseDto<FollowRespDto>.Success("Follow successful.", followDto);
        }

        public async Task<ResponseDto<IEnumerable<FollowRespDto>>> GetFollowersAsync(int userId)
        {
            _logger.LogInformation("Retrieving followers for user ID: {UserId}", userId);
            var follows = await _unitOfWork.Follows.GetAsync(f => f.FolloweeId == userId);
            var followDtos = _mapper.Map<IEnumerable<FollowRespDto>>(follows);

            _logger.LogInformation("Followers retrieved successfully for user ID: {UserId}", userId);
            return ResponseDto<IEnumerable<FollowRespDto>>.Success("Followers retrieved successfully.", followDtos);
        }

        public async Task<ResponseDto<IEnumerable<FollowRespDto>>> GetFollowingAsync(int userId)
        {
            _logger.LogInformation("Retrieving following for user ID: {UserId}", userId);
            var follows = await _unitOfWork.Follows.GetAsync(f => f.FollowerId == userId);
            var followDtos = _mapper.Map<IEnumerable<FollowRespDto>>(follows);

            _logger.LogInformation("Following retrieved successfully for user ID: {UserId}", userId);
            return ResponseDto<IEnumerable<FollowRespDto>>.Success("Following retrieved successfully.", followDtos);
        }
    }
}

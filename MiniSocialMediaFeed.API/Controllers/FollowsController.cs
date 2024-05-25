using Microsoft.AspNetCore.Mvc;
using MiniSocialMediaFeed.Application.Interfaces;
using MiniSocialMediaFeed.Application.Services;

namespace MiniSocialMediaFeed.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FollowsController : ControllerBase
    {
        private readonly IFollowService _followService;

        public FollowsController(IFollowService followService)
        {
            _followService = followService;
        }

        [HttpPost("{followerId}/follow/{followeeId}")]
        public async Task<IActionResult> FollowUser(int followerId, int followeeId)
        {
            var response = await _followService.FollowUserAsync(followerId, followeeId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{userId}/followers")]
        public async Task<IActionResult> GetFollowers(int userId)
        {
            var response = await _followService.GetFollowersAsync(userId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{userId}/following")]
        public async Task<IActionResult> GetFollowing(int userId)
        {
            var response = await _followService.GetFollowingAsync(userId);
            return StatusCode(response.StatusCode, response);
        }
    }
}

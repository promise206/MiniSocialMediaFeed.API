using Microsoft.AspNetCore.Mvc;
using MiniSocialMediaFeed.Application.Dtos.RequestDto;
using MiniSocialMediaFeed.Application.Dtos.ResponseDto;
using MiniSocialMediaFeed.Application.Interfaces;
using System.Net;

namespace MiniSocialMediaFeed.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] PostReqDto post)
        {
            if (post.Content.Length > 140)
            {
                var errorResponse = ResponseDto<PostReqDto>.Fail("Post content exceeds 140 characters.", (int)HttpStatusCode.BadRequest);
                return StatusCode(errorResponse.StatusCode, errorResponse);
            }

            var response = await _postService.CreatePostAsync(post);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("feed")]
        public async Task<IActionResult> GetFeed([FromQuery] int userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var feed = await _postService.GetFeedAsync(userId, pageNumber, pageSize);
            return Ok(feed);
        }
    }
}

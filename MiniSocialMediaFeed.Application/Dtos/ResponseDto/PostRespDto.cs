namespace MiniSocialMediaFeed.Application.Dtos.ResponseDto
{
    public class PostRespDto
    {
        public int Id { get; set; }

        public string Content { get; set; }

        public int Likes { get; set; }

        public DateTime CreatedAt { get; set; }

        public int UserId { get; set; }
    }
}

﻿namespace MiniSocialMediaFeed.Domain.Entities
{
    public class Post
    {
        public int Id { get; set; }

        public string Content { get; set; }

        public int Likes { get; set; }

        public DateTime CreatedAt { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }
    }
}

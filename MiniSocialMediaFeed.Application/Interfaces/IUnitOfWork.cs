using MiniSocialMediaFeed.Domain.Entities;

namespace MiniSocialMediaFeed.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<User> Users { get; }

        IRepository<Post> Posts { get; }

        IRepository<Follow> Follows { get; }

        Task<int> SaveAsync();
    }
}

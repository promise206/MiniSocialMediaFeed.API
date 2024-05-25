using MiniSocialMediaFeed.Application.Interfaces;
using MiniSocialMediaFeed.Domain.Entities;

namespace MiniSocialMediaFeed.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MiniSocialMediaFeedDbContext _context;
        public IRepository<User> Users { get; private set; }
        public IRepository<Post> Posts { get; private set; }
        public IRepository<Follow> Follows { get; private set; }

        public UnitOfWork(MiniSocialMediaFeedDbContext context)
        {
            _context = context;
            Users = new Repository<User>(context);
            Posts = new Repository<Post>(context);
            Follows = new Repository<Follow>(context);
        }

        public async Task<int> SaveAsync() => await _context.SaveChangesAsync();
        public void Dispose() => _context.Dispose();
    }
}

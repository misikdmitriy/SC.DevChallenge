using Microsoft.EntityFrameworkCore;
using SC.DevChallenge.Db.Contexts;
using SC.DevChallenge.Db.Factories.Contracts;

namespace SC.DevChallenge.Db.Factories
{
    public class AppDbContextFactory : IDbContextFactory<AppDbContext>
    {
        private readonly DbContextOptions _options;

        public AppDbContextFactory(DbContextOptions options)
        {
            _options = options;
        }

        public AppDbContext CreateContext()
        {
            return new AppDbContext(_options);
        }
    }
}

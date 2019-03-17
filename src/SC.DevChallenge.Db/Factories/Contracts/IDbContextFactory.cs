using Microsoft.EntityFrameworkCore;

namespace SC.DevChallenge.Db.Factories.Contracts
{
    public interface IDbContextFactory<out TContext>
        where TContext : DbContext
    {
        TContext CreateContext();
    }
}

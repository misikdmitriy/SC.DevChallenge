using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SC.DevChallenge.Db.Contexts;
using SC.DevChallenge.Db.Models.Contracts;
using SC.DevChallenge.Db.Repositories.Contracts;

namespace SC.DevChallenge.Db.Repositories
{
    public class DbRepository<TEntity> : IDbRepository<TEntity>
            where TEntity : class, IIDentifiable
    {
        protected readonly AppDbContext Context;

        public DbRepository(AppDbContext context)
        {
            Context = context;
        }

        public async Task<TEntity[]> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetQuery(Context).Where(predicate).ToArrayAsync();
        }

        public async Task<int> Count(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetQuery(Context).CountAsync(predicate);
        }

        //public async Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        //{
        //    return await GetQuery(Context).SingleOrDefaultAsync(predicate);
        //}

        //public async Task AddAsync(TEntity entity)
        //{
        //    await Context.Set<TEntity>().AddAsync(entity);
        //    await Context.SaveChangesAsync();
        //}

        //public async Task UpdateAsync(TEntity entity)
        //{
        //    Context.Entry(entity).State = EntityState.Modified;
        //    await Context.SaveChangesAsync();
        //}

        protected virtual IQueryable<TEntity> GetQuery(AppDbContext context)
        {
            return context.Set<TEntity>();
        }
    }
}

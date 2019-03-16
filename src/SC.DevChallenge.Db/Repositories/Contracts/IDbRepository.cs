using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SC.DevChallenge.Db.Models.Contracts;

namespace SC.DevChallenge.Db.Repositories.Contracts
{
	public interface IDbRepository<TEntity>
		where TEntity : class, IIDentifiable
	{
		Task<TEntity[]> FindAsync(Expression<Func<TEntity, bool>> predicate);
		Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
		Task AddAsync(TEntity entity);
		Task UpdateAsync(TEntity entity);
	}
}

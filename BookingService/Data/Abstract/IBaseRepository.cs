using System;
using System.Linq;
using System.Threading.Tasks;

namespace BookingService.Data.Abstract
{
    public interface IBaseRepository<TEntity> : IDisposable
        where TEntity : class
    {
        Task<TEntity> FindAsync(params object[] keyValues);
        Task<int> DeleteAsync(TEntity entity);
        Task<TEntity> InsertAsync(TEntity entity);
        IQueryable<TEntity> SelectQuery(string query, params object[] parameters);
        Task<int> UpdateAsync(TEntity entity);
    }
}

using Microsoft.EntityFrameworkCore;

namespace OpenDeepSpaceEFCore
{
    /// <summary>
    /// 仓储 Scoped范围内  
    /// </summary>
    /// <typeparam name="TDbContext">应该是Scoped生命周期的TDbContext</typeparam>
    public interface IRepository<TDbContext,TEntity> where TDbContext:DbContext
    {

        TEntity InsertAsync(TEntity entity);


    }
}

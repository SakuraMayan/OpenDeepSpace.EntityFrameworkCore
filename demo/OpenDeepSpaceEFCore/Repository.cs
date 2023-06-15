using Microsoft.EntityFrameworkCore;

namespace OpenDeepSpaceEFCore
{
    public class Repository<TDbContext,TEntity>:IRepository<TDbContext,TEntity> where TDbContext:DbContext
    {
        private IUnitOfWorkDbContextProvider<TDbContext> _unitOfWorkDbContextProvider;

        public Repository(IUnitOfWorkDbContextProvider<TDbContext> unitOfWorkDbContextProvider) {
        
            _unitOfWorkDbContextProvider = unitOfWorkDbContextProvider;
        }

        public TEntity InsertAsync(TEntity entity)
        {
            return (TEntity)_unitOfWorkDbContextProvider.GetDbContext().Add(entity).Entity;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace OpenDeepSpaceEFCore
{

   

    /// <summary>
    /// 
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {

        public Guid Id { get; set; }

        private ConcurrentDictionary<Guid,DbContext> dbContexts=new ConcurrentDictionary<Guid, DbContext> ();

        /// <summary>
        /// 是否已经提交事务
        /// </summary>
        private bool IsCommited { get; set; }

        /// <summary>
        /// 是否正在提交事务
        /// </summary>
        private bool IsCommiting { get; set; }

        public UnitOfWork()
        {
           Id= Guid.NewGuid ();

        }


        public void AddDbContext(DbContext dbContext)
        {
            if (!dbContexts.ContainsKey(Id))//不包含当前工作单元Id对应的上下文
                dbContexts.TryAdd(Id, dbContext);
            //打开事务
            dbContext.Database.BeginTransaction();
        }

        public void Commit()
        {
            SaveChanges();//暂存

            foreach (var dbContext in dbContexts.Values)
            {
                dbContext.Database.CommitTransaction();
            }
        }

        public void RollBack()
        {
            foreach (var dbContext in dbContexts.Values)
            {
                dbContext.Database.CommitTransaction();
            }
        }

        public void SaveChanges()
        {
            foreach (var dbContext in dbContexts.Values)
            { 
                dbContext.SaveChanges();     
            }
        }

        public DbContext FindDbContext(Guid Id)
        {
            if (!dbContexts.ContainsKey(Id))
                return null;

            return dbContexts[Id];
        }
    }
}

/*=================================================================================================

=====================================Copyright Create Declare Start================================

Copyright © 2023 by OpenDeepSpace. All rights reserved.
Author: OpenDeepSpace	
CreateTime: 2023/6/19 21:34:44	
CLR: 4.0.30319.42000	
Description:


=====================================Copyright Create Declare End==================================

=====================================Modify Records Explain Start==================================

Modifier:	
ModificationTime:
Description:


===================================================================================================

Modifier:
ModificationTime:
Description:


=====================================Modify Records Explain End====================================

===================================================================================================*/

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.EntityFrameworkCore
{
    /// <summary>
    /// 工作单元
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {

        public Dictionary<string,DbContext> dbContexts = new Dictionary<string,DbContext>();

        public Guid Id { get; set; }

        public UnitOfWork()
        {
            Id = Guid.NewGuid();
        }

       
        

        public void CommitTransaction()
        {
            SaveChanges();

            //第一个上下文
            DbContext firstDbContext = dbContexts.First().Value;  

            //提交改变到数据库
            foreach (var context in dbContexts.Values) 
            {
                //如果当前的上下文的连接和第一个上下文连接相同 说明是共享事务
                if (context.Database.GetDbConnection() == firstDbContext.Database.GetDbConnection())
                {
                    continue;
                }

                context.Database.CommitTransaction();//提交事务
                
            }

            //第一个上下文提交事务 共享事务提交事务 如果全部上下文都是共享事务的话 这里保证上面continue之后共享事务的事务提交
            firstDbContext.Database.CommitTransaction();
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            await SaveChangesAsync();

            //提交改变到数据库
            foreach (var context in dbContexts.Values)
            { 
            
            }
        }

        public void RollbackTransaction(CancellationToken cancellationToken = default)
        {
            //第一个上下文
            DbContext firstDbContext = dbContexts.First().Value;  

            //提交改变到数据库
            foreach (var context in dbContexts.Values) 
            {
                //如果当前的上下文的连接和第一个上下文连接相同 说明是共享事务
                if (context.Database.GetDbConnection() == firstDbContext.Database.GetDbConnection())
                {
                    continue;
                }

                context.Database.RollbackTransaction();//回滚事务
                
            }

            //第一个上下文回滚事务 共享事务回滚事务 如果全部上下文都是共享事务的话 这里保证上面continue之后共享事务的回滚事务
            firstDbContext.Database.RollbackTransaction();
        }

        public async Task RollbackTransactionAsync()
        {
            foreach (var context in dbContexts.Values)
            { 
                
            }
        }

        public void AddDbContext(string dbContextKey, DbContext dbContext)
        {
            BeginTransaction(dbContext);
            AddDbContextInternal(dbContextKey, dbContext);

        }


        public async Task AddDbContextAsync(string dbContextKey, DbContext dbContext)
        {
            await BeginTransactionAsync(dbContext);

            AddDbContextInternal(dbContextKey, dbContext);
        }

        private void AddDbContextInternal(string dbContextKey, DbContext dbContext)
        {
            if (!dbContexts.ContainsKey(dbContextKey))//不存在
                dbContexts[dbContextKey] = dbContext;
        }

        /// <summary>
        /// 开始事务
        /// </summary>
        /// <param name="dbContext"></param>
        private void BeginTransaction(DbContext dbContext)
        { 
            dbContext.Database.BeginTransaction();
        }

        /// <summary>
        /// 异步开始事务
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        private async Task BeginTransactionAsync(DbContext dbContext)
        { 
            await dbContext.Database.BeginTransactionAsync();
        }

        /// <summary>
        /// 保存改变
        /// </summary>
        private void SaveChanges()
        {
            foreach (var context in dbContexts.Values)
            { 
                context.SaveChanges();
            }
        }

        /// <summary>
        /// 异步保存改变
        /// </summary>
        private async Task SaveChangesAsync()
        {
            foreach (var context in dbContexts.Values)
            { 
                await context.SaveChangesAsync();
            }
        }

        
    }
}

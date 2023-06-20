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
        //存储上下文
        public Dictionary<string,DbContext> dbContexts = new Dictionary<string,DbContext>();

        //存储事务 采用同数据库连接共享事务的方式
        public Dictionary<string, IDbContextTransaction> dbContextTransactions = new Dictionary<string, IDbContextTransaction>();

       
        public Guid UnitOfWorkId { get; set; }

        public UnitOfWork()
        {
            UnitOfWorkId = Guid.NewGuid();
        }

       
        

        public void Commit()
        {
            //保存改变
            SaveChanges();

            foreach (var transaction in dbContextTransactions.Values)
            {
                //尝试提交事务
                try 
                {
                    transaction.Commit();
                
                }
                catch 
                {//提交事务出现异常回滚 如果都没到这一步就出现异常事务都未提交就不需要回滚了

                    transaction.Rollback();
                }
            }
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            await SaveChangesAsync();

            //提交改变到数据库
            foreach (var context in dbContexts.Values)
            { 
            
            }
        }

        //public void Rollback(CancellationToken cancellationToken = default)
        //{
        //    foreach (var transaction in dbContextTransactions.Values)
        //    {
        //        //回滚事务
        //        transaction.Rollback();
        //    }
        //}

        //public async Task RollbackAsync()
        //{
        //    foreach (var context in dbContexts.Values)
        //    { 
                
        //    }
        //}

        public void AddDbContextWithJudgeTransaction(string dbContextKey, DbContext dbContext)
        {
            BeginTransaction(dbContext);
            AddDbContextInternal(dbContextKey, dbContext);

        }


        public async Task AddDbContextWithJudgeTransactionAsync(string dbContextKey, DbContext dbContext)
        {
            await BeginTransactionAsync(dbContext);

            AddDbContextInternal(dbContextKey, dbContext);
        }

        public DbContext GetDbContext(string dbContextKey)
        {
            if(dbContexts.ContainsKey(dbContextKey))
                return dbContexts[dbContextKey];
            return null;
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
            //同类型 同字符串 不同实例 采用共享事务(通过同连接来共享 或采用同DbContextOptions)
            //不同类型 同字符串  采用共享事务
            //不同类型 不同字符串 这是不同库 不能使用共享事务单独处理事务
            string connectionString = dbContext.Database.GetConnectionString();

            //查找连接字符串可能存在的事务
            IDbContextTransaction existDbContextTransaction = null;
            if (dbContextTransactions.ContainsKey(connectionString))
                existDbContextTransaction = dbContextTransactions[connectionString];

            //找出连接字符串对应的所有加入到工作单元中的上下文 反转找出最近加入的符合条件的一个
            DbContext recentlyConnDbContext = null;
            if (dbContexts.Any())
                recentlyConnDbContext = dbContexts.Where(t => t.Key.Contains(connectionString)).Reverse().First().Value;
           

            if (existDbContextTransaction!=null && recentlyConnDbContext !=null && dbContext.Database.GetDbConnection() == recentlyConnDbContext.Database.GetDbConnection())
            { //如果 存在上下文事务 且 当前加入上下文和最近加入的同一个连接字符串的上下文是属于同一连接 则共享事务
                
                dbContext.Database.UseTransaction(existDbContextTransaction.GetDbTransaction());

                return;
            }


            ///开启事务 手动开启事务之后 efcore默认的自动事务将失效
            ///根据<see cref="DbContext.Database.AutoTransactionsEnabled"/> 解释该值设置为false或使用了<see cref="DbContext.Database.BeginTransaction"/> efcore在调用SaveChanges是自动事务将不起作用
            IDbContextTransaction dbContextTransaction = dbContext.Database.BeginTransaction();

            //事务加入到工作单元中 这里如果多个不同的上下文实例 同一字符串没使用共享连接 加入可能出现异常
            dbContextTransactions.Add(connectionString, dbContextTransaction);
            

            
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
                //尝试执行数据库保存改变
                try
                {
                    context.SaveChanges();//数据出问题会出异常
                }
                catch //捕获到数据库保存改变异常
                {
                    
                }
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

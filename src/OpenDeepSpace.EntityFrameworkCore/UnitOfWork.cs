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
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.EntityFrameworkCore
{
    /// <summary>
    /// 工作单元
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private ILogger<UnitOfWork> logger;

        //存储上下文
        public Dictionary<string,DbContext> dbContexts = new Dictionary<string,DbContext>();

        ///存储事务 采用同数据库同连接共享事务的方式 键为连接串这种有一个问题
        ///就是共享连接之后通过<see cref="DbContext.Database.GetDbConnectionString()"></see>获取的字符串可能会由于字符串连接被格式化了导致不能进行比较操作
        ///故采用DbConnection作为键 也符合理念 即通过共享数据库连接 来共享事务
        //public Dictionary<string, IDbContextTransaction> dbContextTransactions = new Dictionary<string, IDbContextTransaction>();
        public Dictionary<DbConnection, IDbContextTransaction> dbContextTransactions = new Dictionary<DbConnection, IDbContextTransaction>();

        //数据库上下文对应事务所处状态
        public Dictionary<IDbContextTransaction,DbContextTransactionStatus> dbContextTransactionsStatus = new Dictionary<IDbContextTransaction,DbContextTransactionStatus>();
       
        //失败事务的数据库上下文
        public List<DbContext> failTransactionalDbContexts=new List<DbContext>();

        public Guid UnitOfWorkId { get; set; }

        ///工作单元选项不允许设置 只能通过<see cref="Initialize(IUnitOfWorkOptions)"/>来初始化 且一个相同工作单元实例只能初始化一次
        public IUnitOfWorkOptions UnitOfWorkOptions { get; private set; }

        public UnitOfWork(ILogger<UnitOfWork> logger)
        {
            UnitOfWorkId = Guid.NewGuid();
            this.logger = logger;
        }

        public void Initialize(IUnitOfWorkOptions unitOfWorkOptions)
        {
            if (UnitOfWorkOptions == null)//工作单元选项未初始化 才初始化
                UnitOfWorkOptions = unitOfWorkOptions;
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
                    
                    //事务已回滚 或已提交 就直接返回 不需要在提交
                    if (dbContextTransactionsStatus[transaction] == DbContextTransactionStatus.RolledBack || dbContextTransactionsStatus[transaction] == DbContextTransactionStatus.Commited)
                        return;


                    transaction.Commit();
                    //记录事务为已提交状态
                    dbContextTransactionsStatus[transaction] = DbContextTransactionStatus.Commited;
                
                }
                catch (Exception ex)
                {//提交事务出现异常回滚 如果都没到这一步就出现异常事务都未提交就不需要回滚了

                    //输出日志
                    logger.LogError($"{typeof(UnitOfWork).FullName}-{UnitOfWorkId}-{nameof(Commit)}-{DateTime.Now}:{ex.Message}{ex.StackTrace}{ex.InnerException?.Message}{ex.InnerException?.StackTrace}");

                    //回滚事务并记录事务状态
                    RollBackInternal(transaction);
                }
            }
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            await SaveChangesAsync();

            //提交改变到数据库
            foreach (var transaction in dbContextTransactions.Values)
            {
                //尝试提交事务
                try
                {

                    //事务已回滚 或已提交 就直接返回 不需要在提交
                    if (dbContextTransactionsStatus[transaction] == DbContextTransactionStatus.RolledBack || dbContextTransactionsStatus[transaction] == DbContextTransactionStatus.Commited)
                        return;


                    await transaction.CommitAsync();
                    //记录事务为已提交状态
                    dbContextTransactionsStatus[transaction] = DbContextTransactionStatus.Commited;

                }
                catch (Exception ex)
                {//提交事务出现异常回滚 如果都没到这一步就出现异常事务都未提交就不需要回滚了

                    //输出日志
                    logger.LogError($"{typeof(UnitOfWork).FullName}-{UnitOfWorkId}-{nameof(CommitAsync)}-{DateTime.Now}:{ex.Message}{ex.StackTrace}{ex.InnerException?.Message}{ex.InnerException?.StackTrace}");

                    //回滚事务并记录事务状态
                    await RollBackInternalAsync(transaction);
                }
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

        /// <summary>
        /// 回滚事务 并记录对应事务状态
        /// </summary>
        /// <param name="dbContextTransaction"></param>
        private void RollBackInternal(IDbContextTransaction dbContextTransaction)
        {


            ///回滚事务 如果事务已回滚 在后续执行出现的错误中就不再回滚了 即使后续的同连接上下文存在SaveChanges操作
            ///上面提交事务<see cref="Commit"/>的时候也不会在提交对应的事务 也不会执行成功
            if (dbContextTransactionsStatus[dbContextTransaction] == DbContextTransactionStatus.RolledBack)
                return;
            try
            { //尝试回滚
                dbContextTransaction.Rollback();
            }
            catch(Exception ex)
            {
                //输出日志
                logger.LogError($"{typeof(UnitOfWork).FullName}-{UnitOfWorkId}-{nameof(RollBackInternal)}-{DateTime.Now}:{ex.Message}{ex.StackTrace}{ex.InnerException?.Message}{ex.InnerException?.StackTrace}");
            }

            //事务状态改变为已回滚
            dbContextTransactionsStatus[dbContextTransaction] = DbContextTransactionStatus.RolledBack;
        }

        private async Task RollBackInternalAsync(IDbContextTransaction dbContextTransaction)
        {
            ///回滚事务 如果事务已回滚 在后续执行出现的错误中就不再回滚了 即使后续的同连接上下文存在SaveChanges操作
            ///上面提交事务<see cref="Commit"/>的时候也不会在提交对应的事务 也不会执行成功
            if (dbContextTransactionsStatus[dbContextTransaction] == DbContextTransactionStatus.RolledBack)
                return;
            try
            { //尝试回滚
                await dbContextTransaction.RollbackAsync();
            }
            catch (Exception ex)
            {
                //输出日志
                logger.LogError($"{typeof(UnitOfWork).FullName}-{UnitOfWorkId}-{nameof(RollBackInternalAsync)}-{DateTime.Now}:{ex.Message}{ex.StackTrace}{ex.InnerException?.Message}{ex.InnerException?.StackTrace}");
            }

            //事务状态改变为已回滚
            dbContextTransactionsStatus[dbContextTransaction] = DbContextTransactionStatus.RolledBack;
        }

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
            //工作单元是否存在超时设置 如果当前数据库CommandTimeout设置为空 才进行设置
            if(UnitOfWorkOptions!=null && UnitOfWorkOptions.Timeout.HasValue
                && dbContext.Database.GetCommandTimeout()==null
                )
                dbContext.Database.SetCommandTimeout(UnitOfWorkOptions.Timeout.Value);

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


            //注意:这里比较连接字符串相等是不合适的 共享事务之后通过这种方式获取出的连接字符串有可能是被系统格式化了的
            //比如：
            //Server=localhost;User ID=root;Port=3306;Database=ods;Allow User Variables=True;Ignore Command Transaction=True
            //server = localhost; uid = root; pwd = wy.023; port = 3306; database = ods; Allow User Variables = True; IgnoreCommandTransaction = true
            //解决方法除非找到能相互格式化的方法
            //string connectionString = dbContext.Database.GetConnectionString();

            ////查找连接字符串可能存在的事务
            //IDbContextTransaction existDbContextTransaction = null;
            //if (dbContextTransactions.ContainsKey(connectionString))
            //    existDbContextTransaction = dbContextTransactions[connectionString];

            ////找出连接字符串对应的所有加入到工作单元中的上下文 反转找出最近加入的符合条件的一个
            //DbContext recentlyConnDbContext = null;
            //if (dbContexts.Any())
            //    recentlyConnDbContext = dbContexts.Where(t => t.Key.Contains(connectionString)).Reverse().First().Value;


            //if (existDbContextTransaction!=null && recentlyConnDbContext !=null && dbContext.Database.GetDbConnection() == recentlyConnDbContext.Database.GetDbConnection())
            //{ //如果 存在上下文事务 且 当前加入上下文和最近加入的同一个连接字符串的上下文是属于同一连接 则共享事务

            //    dbContext.Database.UseTransaction(existDbContextTransaction.GetDbTransaction());

            //    return;
            //}
            /////开启事务 手动开启事务之后 efcore默认的自动事务将失效
            /////根据<see cref="DbContext.Database.AutoTransactionsEnabled"/> 解释该值设置为false或使用了<see cref="DbContext.Database.BeginTransaction"/> efcore在调用SaveChanges是自动事务将不起作用
            //IDbContextTransaction dbContextTransaction = dbContext.Database.BeginTransaction();

            ////事务加入到工作单元中 这里如果多个不同的上下文实例 同一字符串没使用共享连接 加入可能出现异常
            //dbContextTransactions.Add(connectionString, dbContextTransaction);

            //========采用新的=============


            ///如果工作单元选项存在并不需要开启事务就不执行手动事务开启 
            ///事务将是EFCORE默认事务(默认事务没有调用<see cref="DbContext.Database.AutoTransactionsEnabled"/>设置为false来进行关闭此时进行默认事务)
            ///然又由于提交<see cref="Commit"/>事务的时候 执行<see cref="SaveChanges"/>是按照每个上下文实例循环调用<see cref="DbContext.SaveChanges()"/>也就是说每调用一次就执行一次默认事务并提交数据到数据库
            ///因此每一个数据库实例的操作，即每一<see cref="Repository{TDbContext, TEntity}"/>的方法调用是处于一个单独的事务
            if (UnitOfWorkOptions != null && UnitOfWorkOptions.IsTransactional == false)
            { 
                dbContext.Database.AutoTransactionsEnabled = false;//不开启事务 连手动事务都关闭
                return;
            }
            

            //工作单元选项为空 或 工作单元选项不为空且需要开启事务 才开启事务


            DbConnection dbConnection = dbContext.Database.GetDbConnection();

            //查找对应的DbConnection是否存在事务 存在就表示是相同的DbConnection连接 那么采用共享事务 否则连接生成新事务

            if (dbContextTransactions.ContainsKey(dbConnection))//同连接 存在事务 就共享事务
            { 
                dbContext.Database.UseTransaction(dbContextTransactions[dbConnection].GetDbTransaction());
                return;
            }

                


            ///开启事务 手动开启事务之后 efcore默认的自动事务将失效
            ///根据<see cref="DbContext.Database.AutoTransactionsEnabled"/> 解释该值设置为false或使用了<see cref="DbContext.Database.BeginTransaction"/> efcore在调用SaveChanges是自动事务将不起作用
            ///如果隔离级别有值 就需要启动有隔离级别的事务
            IDbContextTransaction dbContextTransaction = UnitOfWorkOptions!=null && UnitOfWorkOptions.IsolationLevel.HasValue ?
                    dbContext.Database.BeginTransaction(UnitOfWorkOptions.IsolationLevel.Value): dbContext.Database.BeginTransaction();

            //记录事务状态
            dbContextTransactionsStatus[dbContextTransaction] = DbContextTransactionStatus.Started;

            //事务加入到工作单元中 这里如果多个不同的上下文实例 同一字符串没使用共享连接 加入可能出现异常
            dbContextTransactions.Add(dbConnection, dbContextTransaction);





        }

        /// <summary>
        /// 异步开始事务
        /// </summary>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        private async Task BeginTransactionAsync(DbContext dbContext)
        {
            ///如果工作单元选项存在并不需要开启事务就不执行手动事务开启 
            ///事务将是EFCORE默认事务(默认事务没有调用<see cref="DbContext.Database.AutoTransactionsEnabled"/>设置为false来进行关闭此时进行默认事务)
            ///然又由于提交<see cref="Commit"/>事务的时候 执行<see cref="SaveChanges"/>是按照每个上下文实例循环调用<see cref="DbContext.SaveChanges()"/>也就是说每调用一次就执行一次默认事务并提交数据到数据库
            ///因此每一个数据库实例的操作，即每一<see cref="Repository{TDbContext, TEntity}"/>的方法调用是处于一个单独的事务
            if (UnitOfWorkOptions != null && UnitOfWorkOptions.IsTransactional == false)
            {
                dbContext.Database.AutoTransactionsEnabled = false;//不开启事务 连手动事务都关闭
                return;
            }


            //工作单元选项为空 或 工作单元选项不为空且需要开启事务 才开启事务


            DbConnection dbConnection = dbContext.Database.GetDbConnection();

            //查找对应的DbConnection是否存在事务 存在就表示是相同的DbConnection连接 那么采用共享事务 否则连接生成新事务

            if (dbContextTransactions.ContainsKey(dbConnection))//同连接 存在事务 就共享事务
            {
                await dbContext.Database.UseTransactionAsync(dbContextTransactions[dbConnection].GetDbTransaction());
                return;
            }




            ///开启事务 手动开启事务之后 efcore默认的自动事务将失效
            ///根据<see cref="DbContext.Database.AutoTransactionsEnabled"/> 解释该值设置为false或使用了<see cref="DbContext.Database.BeginTransactionAsync"/> efcore在调用SaveChanges是自动事务将不起作用
            ///如果隔离级别有值 就需要启动有隔离级别的事务
            IDbContextTransaction dbContextTransaction = UnitOfWorkOptions != null && UnitOfWorkOptions.IsolationLevel.HasValue ?
                    await dbContext.Database.BeginTransactionAsync(UnitOfWorkOptions.IsolationLevel.Value) : await dbContext.Database.BeginTransactionAsync();

            //记录事务状态
            dbContextTransactionsStatus[dbContextTransaction] = DbContextTransactionStatus.Started;

            //事务加入到工作单元中 这里如果多个不同的上下文实例 同一字符串没使用共享连接 加入可能出现异常
            dbContextTransactions.Add(dbConnection, dbContextTransaction);
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
                    //如果存在失败的上下文集合 且当前的上下文连接与失败的上下文连接一致 就不在保存数据
                    if (failTransactionalDbContexts.Exists(t => t.Database.GetDbConnection() == context.Database.GetDbConnection()))
                        continue;

                    context.SaveChanges();//数据出问题会出异常
                }
                catch(Exception ex) //捕获到数据库保存改变异常
                {
                    //输出日志
                    logger.LogError($"{typeof(UnitOfWork).FullName}-{UnitOfWorkId}-{nameof(SaveChanges)}-{DateTime.Now}:{ex.Message}{ex.StackTrace}{ex.InnerException?.Message}{ex.InnerException?.StackTrace}");

                    //如果是需要事务 记录失败的上下文
                    if (UnitOfWorkOptions == null || (UnitOfWorkOptions != null && UnitOfWorkOptions.IsTransactional))
                        failTransactionalDbContexts.Add(context);

                    //回滚事务 并记录事务所处状态
                    //RollBackInternal(context.Database.CurrentTransaction);//通过context.Database.CurrentTransaction获取出的当前事务与记录的事务不一样即使共享事务所以我们直接通过连接从事务字典中取
                    if (dbContextTransactions.ContainsKey(context.Database.GetDbConnection()))//存在当前连接的事务才进行异常回滚 否则不会滚 出现这种情况是不使用手动管理事务 默认事务的时候
                        RollBackInternal(dbContextTransactions[context.Database.GetDbConnection()]);
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
                //尝试执行数据库保存改变
                try
                {
                    //如果存在失败的上下文集合 且当前的上下文连接与失败的上下文连接一致 就不在保存数据
                    if (failTransactionalDbContexts.Exists(t=>t.Database.GetDbConnection()==context.Database.GetDbConnection()))
                        continue;

                    await context.SaveChangesAsync();//数据出问题会出异常
                }
                catch (Exception ex) //捕获到数据库保存改变异常
                {
                    //输出日志
                    logger.LogError($"{typeof(UnitOfWork).FullName}-{UnitOfWorkId}-{nameof(SaveChangesAsync)}-{DateTime.Now}:{ex.Message}{ex.StackTrace}{ex.InnerException?.Message}{ex.InnerException?.StackTrace}");

                    //如果是需要事务 记录失败的上下文
                    if (UnitOfWorkOptions==null || (UnitOfWorkOptions != null && UnitOfWorkOptions.IsTransactional))
                        failTransactionalDbContexts.Add(context);

                    //回滚事务 并记录事务所处状态
                    //RollBackInternal(context.Database.CurrentTransaction);//通过context.Database.CurrentTransaction获取出的当前事务与记录的事务不一样即使共享事务所以我们直接通过连接从事务字典中取
                    if (dbContextTransactions.ContainsKey(context.Database.GetDbConnection()))//存在当前连接的事务才进行异常回滚 否则不会滚 出现这种情况是不使用手动管理事务 默认事务的时候
                        RollBackInternal(dbContextTransactions[context.Database.GetDbConnection()]);
                }
            }
        }

        
    }
}

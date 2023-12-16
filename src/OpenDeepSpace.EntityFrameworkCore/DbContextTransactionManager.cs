/*=================================================================================================

=====================================Copyright Create Declare Start================================

Copyright © 2023 by OpenDeepSpace. All rights reserved.
Author: OpenDeepSpace	
CreateTime: 2023/12/16 12:26:27	
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
    /// 数据库上下文事务管理
    /// </summary>
    public class DbContextTransactionManager : IDbContextTransactionManager
    {

        public List<DbContext> dbContexts=new List<DbContext>();
        //记录数据库对应的事务
        public Dictionary<DbContext,IDbContextTransaction> dbContextTransactionDic=new Dictionary<DbContext, IDbContextTransaction>();
        //记录事务对应的状态
        public Dictionary<IDbContextTransaction, DbContextTransactionStatus> dbContextTransactionStatusDic = new Dictionary<IDbContextTransaction, DbContextTransactionStatus>();

        public void AddDbContextWithBeginTransaction(DbContext dbContext)
        {
            IDbContextTransaction dbContextTransaction = null;

            foreach (var _dbContext in dbContexts)
            {

                //共享连接 那么就共享事务
                if (_dbContext.Database.GetDbConnection() == dbContext.Database.GetDbConnection())
                {
                    dbContextTransaction = dbContextTransactionDic[_dbContext];
                    dbContext.Database.UseTransaction(dbContextTransaction.GetDbTransaction());
                    break;
                }

            }

            //开启手动管理事务
            if(dbContextTransaction == null)
                dbContextTransaction =  dbContext.Database.BeginTransaction();

            //事务加入到数据库对应事务的字典中
            dbContextTransactionDic[dbContext] = dbContextTransaction;

            //记录当前事务状态
            dbContextTransactionStatusDic[dbContextTransaction] = DbContextTransactionStatus.Started;
            
            //当前上下文加入
            dbContexts.Add(dbContext);
        }

        public async Task AddDbContextWithBeginTransactionAsync(DbContext dbContext,CancellationToken cancellationToken=default)
        {
            IDbContextTransaction dbContextTransaction = null;

            foreach (var _dbContext in dbContexts)
            {

                //共享连接 那么就共享事务
                if (_dbContext.Database.GetDbConnection() == dbContext.Database.GetDbConnection())
                {
                    dbContextTransaction = dbContextTransactionDic[_dbContext];
                    await dbContext.Database.UseTransactionAsync(dbContextTransaction.GetDbTransaction(),cancellationToken);
                    break;
                }

            }

            //开启手动管理事务
            if (dbContextTransaction == null)
                dbContextTransaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

            //事务加入到数据库对应事务的字典中
            dbContextTransactionDic[dbContext] = dbContextTransaction;

            //记录当前事务状态
            dbContextTransactionStatusDic[dbContextTransaction] = DbContextTransactionStatus.Started;

            //当前上下文加入
            dbContexts.Add(dbContext);
        }

        public void Commit()
        {
            CommitInternal(dbContextTransactionDic.Values);
        }

        private void CommitInternal(IEnumerable<IDbContextTransaction> dbContextTransactions)
        {
            foreach (var dbContextTransactional in dbContextTransactions)
            {
                //当前事务已经提交或回滚
                if (dbContextTransactionStatusDic[dbContextTransactional] == DbContextTransactionStatus.Commited
                    ||
                    dbContextTransactionStatusDic[dbContextTransactional] == DbContextTransactionStatus.RolledBack
                    )
                    continue;

                try
                {

                    //尝试提交事务
                    dbContextTransactional.Commit();
                    //改变事务状态为已提交
                    dbContextTransactionStatusDic[dbContextTransactional] = DbContextTransactionStatus.Commited;

                }
                catch
                { //出现异常回滚事务


                    try
                    {
                        //尝试回滚事务
                        dbContextTransactional.Rollback();
                        //改变事务状态为已回滚
                        dbContextTransactionStatusDic[dbContextTransactional] = DbContextTransactionStatus.RolledBack;
                    }
                    catch
                    {
                        dbContextTransactionStatusDic[dbContextTransactional] = DbContextTransactionStatus.Failed;
                    }


                }

            }
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            await CommitInternalAsync(dbContextTransactionDic.Values,cancellationToken);
        }

        private async Task CommitInternalAsync(IEnumerable<IDbContextTransaction> dbContextTransactions,CancellationToken cancellationToken=default)
        {
            foreach (var dbContextTransactional in dbContextTransactions)
            {
                //当前事务已经提交或回滚
                if (dbContextTransactionStatusDic[dbContextTransactional] == DbContextTransactionStatus.Commited
                    ||
                    dbContextTransactionStatusDic[dbContextTransactional] == DbContextTransactionStatus.RolledBack
                    )
                    continue;

                try
                {

                    //尝试提交事务
                    await dbContextTransactional.CommitAsync(cancellationToken);
                    //改变事务状态为已提交
                    dbContextTransactionStatusDic[dbContextTransactional] = DbContextTransactionStatus.Commited;

                }
                catch
                { //出现异常回滚事务


                    try
                    {
                        //尝试回滚事务
                        await dbContextTransactional.RollbackAsync(cancellationToken);
                        //改变事务状态为已回滚
                        dbContextTransactionStatusDic[dbContextTransactional] = DbContextTransactionStatus.RolledBack;
                    }
                    catch
                    {
                        dbContextTransactionStatusDic[dbContextTransactional] = DbContextTransactionStatus.Failed;
                    }


                }

            }
        }

        public void Commit(params DbContext[] dbContexts)
        {
            //筛选出符合的数据库上下文事务字典集合
            IEnumerable<IDbContextTransaction> dbContextTransactions = dbContextTransactionDic.Where(kv => dbContexts.ToList().Exists(db=>db == kv.Key)).Select(t=>t.Value);
            CommitInternal(dbContextTransactions);
            
        }

        public void Commit(IEnumerable<DbContext> dbContexts)
        {
            Commit(dbContexts.ToArray());
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default, params DbContext[] dbContexts)
        {
            await CommitAsync(dbContexts.ToArray(), cancellationToken);
        }

        public async Task CommitAsync(IEnumerable<DbContext> dbContexts, CancellationToken cancellationToken = default)
        {
            IEnumerable<IDbContextTransaction> dbContextTransactions = dbContextTransactionDic.Where(kv => dbContexts.ToList().Exists(db => db == kv.Key)).Select(t => t.Value);
            await CommitInternalAsync(dbContextTransactions, cancellationToken);
        }
    }
}

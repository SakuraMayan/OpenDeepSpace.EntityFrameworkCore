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

        public void Commit()
        {
            foreach (var dbContextTransactional in dbContextTransactionDic.Values)
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

        public void Commit(params DbContext[] dbContexts)
        {
            throw new NotImplementedException();
        }

        public void Commit(IEnumerable<DbContext> dbContexts)
        {
            throw new NotImplementedException();
        }
    }
}

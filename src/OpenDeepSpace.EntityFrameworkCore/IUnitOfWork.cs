/*=================================================================================================

=====================================Copyright Create Declare Start================================

Copyright © 2023 by OpenDeepSpace. All rights reserved.
Author: OpenDeepSpace	
CreateTime: 2023/6/19 21:13:55	
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.EntityFrameworkCore
{

    /// <summary>
    /// 工作单元接口
    /// </summary>
    public interface IUnitOfWork
    {
        public Guid UnitOfWorkId { get; }

        /// <summary>
        /// 提交事务
        /// </summary>
        void Commit();

        /// <summary>
        /// 异步提交事务
        /// </summary>
        Task CommitAsync(CancellationToken cancellationToken=default);


        /// <summary>
        /// 回滚事务
        /// </summary>
        //void Rollback(CancellationToken cancellationToken = default);

        /// <summary>
        /// 异步回滚事务
        /// </summary>
        //Task RollbackAsync();

        /// <summary>
        /// 添加上下文到工作单元中
        /// 且包含事务的开启和共享操作
        /// </summary>
        /// <param name="dbContextKey"></param>
        /// <param name="dbContext"></param>
        void AddDbContextWithJudgeTransaction(string dbContextKey,DbContext dbContext);

        /// <summary>
        /// 异步添加上下文到工作单元中
        /// 且包含事务的开启和共享操作
        /// </summary>
        /// <param name="dbContextKey"></param>
        /// <param name="dbContext"></param>
        Task AddDbContextWithJudgeTransactionAsync(string dbContextKey,DbContext dbContext);

        /// <summary>
        /// 获取工作单元中存在的上下文
        /// </summary>
        /// <param name="dbContextKey"></param>
        /// <returns></returns>
        DbContext GetDbContext(string dbContextKey);

    }
}

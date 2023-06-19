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

        /// <summary>
        /// 提交事务
        /// </summary>
        void CommitTransaction();

        /// <summary>
        /// 异步提交事务
        /// </summary>
        Task CommitTransactionAsync(CancellationToken cancellationToken=default);


        /// <summary>
        /// 回滚事务
        /// </summary>
        void RollbackTransaction(CancellationToken cancellationToken = default);

        /// <summary>
        /// 异步回滚事务
        /// </summary>
        Task RollbackTransactionAsync();

        /// <summary>
        /// 添加上下文当工作单元中
        /// </summary>
        /// <param name="dbContextKey"></param>
        /// <param name="dbContext"></param>
        void AddDbContext(string dbContextKey,DbContext dbContext);

        /// <summary>
        /// 异步添加上下文到工作单元中
        /// </summary>
        /// <param name="dbContextKey"></param>
        /// <param name="dbContext"></param>
        Task AddDbContextAsync(string dbContextKey,DbContext dbContext);

    }
}

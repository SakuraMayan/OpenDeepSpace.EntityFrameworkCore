/*=================================================================================================

=====================================Copyright Create Declare Start================================

Copyright © 2023 by OpenDeepSpace. All rights reserved.
Author: OpenDeepSpace	
CreateTime: 2023/12/16 11:56:38	
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
    /// 数据库事务管理者
    /// </summary>
    public interface IDbContextTransactionManager
    {

        /// <summary>
        /// 添加数据库上下文并开启事务
        /// </summary>
        void AddDbContextWithBeginTransaction(DbContext dbContext);

        /// <summary>
        /// 提交所有数据库事务
        /// </summary>
        void Commit();

        /// <summary>
        /// 提交事务
        /// </summary>
        /// <param name="dbContexts">指定提交哪些数据库的事务</param>
        void Commit(params DbContext[] dbContexts);

        /// <summary>
        /// 提交事务
        /// </summary>
        /// <param name="dbContexts">指定提交事务的数据库的集合</param>
        void Commit(IEnumerable<DbContext> dbContexts);
    }
}

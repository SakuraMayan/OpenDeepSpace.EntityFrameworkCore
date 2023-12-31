﻿/*=================================================================================================

=====================================Copyright Create Declare Start================================

Copyright © 2023 by OpenDeepSpace. All rights reserved.
Author: OpenDeepSpace	
CreateTime: 2023/6/20 23:12:15	
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.EntityFrameworkCore
{
    /// <summary>
    /// 数据库上下文事务状态
    /// </summary>
    public enum DbContextTransactionStatus
    {
        /// <summary>
        /// 已开始事务
        /// </summary>
        Started,

        /// <summary>
        /// 已提交事务
        /// </summary>
        Commited,

        /// <summary>
        /// 已回滚事务
        /// </summary>
        RolledBack,
        
        /// <summary>
        /// 事务失败 提交失败 回滚也失败
        /// </summary>
        Failed

    }
}

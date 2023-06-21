/*=================================================================================================

=====================================Copyright Create Declare Start================================

Copyright © 2023 by OpenDeepSpace. All rights reserved.
Author: OpenDeepSpace	
CreateTime: 2023/6/19 22:21:59	
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
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.EntityFrameworkCore
{

    /// <summary>
    /// 工作单元选项接口
    /// 涉及对数据库隔离级别 超时的设置
    /// </summary>
    public interface IUnitOfWorkOptions
    {

        /// <summary>
        /// 是否开启事务 主要指的是否手动管理事务 如果不开启手动管理事务 那么就是EFCORE默认事务 即SaveChanges一次就提交一次事务 
        /// 或把EFCORE默认事务也关闭了 SaveChanges调用一次也会保存到数据库
        /// </summary>
        bool IsTransactional { get; }

        /// <summary>
        /// 数据库隔离级别 针对开始事务阶段隔离级别的设置
        /// </summary>
        IsolationLevel? IsolationLevel { get; }

        /// <summary>
        /// 数据库超时时间 毫秒为单位 针对DbContext初始化后的CommandTimeout命令超时设置
        /// </summary>
        int? Timeout { get; }
    }
}

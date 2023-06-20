/*=================================================================================================

=====================================Copyright Create Declare Start================================

Copyright © 2023 by OpenDeepSpace. All rights reserved.
Author: OpenDeepSpace	
CreateTime: 2023/6/19 21:15:11	
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
    /// 工作单元数据库上下文提供者
    /// </summary>
    public interface IUnitOfWorkDbContextProvider<TDbContext> where TDbContext:DbContext
    {

        /// <summary>
        /// 获取上下文
        /// </summary>
        /// <returns></returns>
        TDbContext GetDbContext();

        /// <summary>
        /// 异步获取上下文
        /// </summary>
        /// <returns></returns>
        Task<TDbContext> GetDbContextAsync();

    }


}

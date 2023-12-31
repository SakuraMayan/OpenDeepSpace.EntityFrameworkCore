﻿/*=================================================================================================

=====================================Copyright Create Declare Start================================

Copyright © 2023 by OpenDeepSpace. All rights reserved.
Author: OpenDeepSpace	
CreateTime: 2023/12/16 11:47:10	
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
    public interface IDbContextProvider<TDbContext> where TDbContext:DbContext
    {
        /// <summary>
        /// 获取上下文
        /// </summary>
        /// <returns></returns>
        TDbContext GetDbContext();

        /// <summary>
        /// 获取上下文
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TDbContext> GetDbContextAsync(CancellationToken cancellationToken=default(CancellationToken));
    }
}

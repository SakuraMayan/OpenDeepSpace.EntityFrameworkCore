/*=================================================================================================

=====================================Copyright Create Declare Start================================

Copyright © 2023 by OpenDeepSpace. All rights reserved.
Author: OpenDeepSpace	
CreateTime: 2023/12/16 12:13:33	
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
    /// 数据库上下文提供者
    /// </summary>
    public class DbContextProvider<TDbContext> : IDbContextProvider<TDbContext> where TDbContext : DbContext
    {

        IDbContextTransactionManager transactionManager;

        TDbContext _dbContext;

        public DbContextProvider(TDbContext dbContext, IDbContextTransactionManager transactionManager)
        {
            _dbContext = dbContext;
            this.transactionManager = transactionManager;
        }

        public TDbContext GetDbContext()
        {
            transactionManager.AddDbContextWithBeginTransaction(_dbContext);

            return _dbContext;
        }
    }
}

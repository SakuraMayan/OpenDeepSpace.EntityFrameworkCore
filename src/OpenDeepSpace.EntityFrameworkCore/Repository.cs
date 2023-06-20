/*=================================================================================================

=====================================Copyright Create Declare Start================================

Copyright © 2023 by OpenDeepSpace. All rights reserved.
Author: OpenDeepSpace	
CreateTime: 2023/6/20 17:31:17	
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
    public class Repository<TDbContext, TEntity> : IRepository<TDbContext, TEntity> where TDbContext : DbContext where TEntity : class 
    {
        public Repository(IUnitOfWorkDbContextProvider<TDbContext> unitOfWorkDbContextProvider)
        {
            _unitOfWorkDbContextProvider = unitOfWorkDbContextProvider;
        }

        //注入工作单元上下文提供者 获取加入到工作单元的上下文即获取与工作单元绑定的上下文
        public IUnitOfWorkDbContextProvider<TDbContext> _unitOfWorkDbContextProvider { get; set; }

        public TEntity Insert(TEntity entity)
        {
            ///此处没有调用SaveChanges在工作单元提交的时候调用 <see cref="UnitOfWork.Commit"/>
            ///如果没有开启手动管理事务且<see cref="DbContext.Database.AutoTransactionsEnabled"/>的值为true即开启自动事务
            ///每调用一次<see cref="UnitOfWork.Commit"/>执行SaveChanges就会提交一次事务 
            ///使用的是EFCore的默认事务即每一次SaveChanges就是一次单独的事务
            ///如果没有开启手动管理事务且<see cref="DbContext.Database.AutoTransactionsEnabled"/>的值为false就没有事务，执行一次SaveChanges会自动写入数据到数据库
            return _unitOfWorkDbContextProvider.GetDbContext().Add(entity).Entity;
        }
    }
}

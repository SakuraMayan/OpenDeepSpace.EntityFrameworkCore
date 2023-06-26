/*=================================================================================================

=====================================Copyright Create Declare Start================================

Copyright © 2023 by OpenDeepSpace. All rights reserved.
Author: OpenDeepSpace	
CreateTime: 2023/6/20 17:44:30	
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
    public class UnitOfWorkDbContextProvider<TDbContext> : IUnitOfWorkDbContextProvider<TDbContext> where TDbContext : DbContext
    {
        private TDbContext _context;

        private IUnitOfWork _unitOfWork;

        public UnitOfWorkDbContextProvider(TDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        public TDbContext GetDbContext()
        {
            ///查看当前工作单元中是否存在相同的数据库上下文DbContext
            ///这里相同的数据库上下文DbContext 
            ///指的是同一个DbContext类型<see cref="typeof(TDbContext).FullName"/> 
            ///且同一个连接字符串<see cref="DbContext.Database.GetConnectionString()"/>  
            ///且是同一个实例<see cref="DbContext.ContextId.InstanceId"/> 才表示是相同的
            ///想想：如果已经是同一个实例<see cref="DbContext.ContextId.InstanceId"/> 那么肯定上下文类型和连接字符串肯定相同 所以下面的内容修改成实例Id即可
            ///通过共享连接之后 获取出来的<see cref="_context.Database.GetConnectionString()"/>是被格式化了的与最初的可能不一样
            ///string dbContextKey = typeof(TDbContext).FullName + "&&" + _context.Database.GetConnectionString() + "&&" + _context.ContextId.InstanceId;
            ///注意：这里以实例Id作为Key之后 那么相同上下文在<see cref="IUnitOfWorkOptions.IsTransactional"/>设置为false时，实际指的不开启手动事务，efcore默认事务还是在
            ///由于同一个上下文最后统一进行SaveChanges 那么还是以事务的形式提交 如果Key采用上面的类型+连接字符串+实例Id 那么就会出现同Key所对应的上下文在UnitOfWork中是采用一次SaveChanges然后采用默认事务
            string dbContextKey = _context.ContextId.InstanceId.ToString();
            if (_unitOfWork.GetDbContext(dbContextKey) != null) //完全相同的直接返回
                return (TDbContext)_unitOfWork.GetDbContext(dbContextKey);

            //不完全相同的情况

            //同类型 同字符串 不同实例 采用共享事务
            //不同类型 同字符串  采用共享事务
            //不同类型 不同字符串 这是不同库 不能使用共享事务单独处理事务

            //加入到工作单元中并判断是否开启事务以及如何开启事务
            _unitOfWork.AddDbContextWithJudgeTransaction(dbContextKey, _context);

            return _context;
        }

        public async Task<TDbContext> GetDbContextAsync()
        {

            string dbContextKey = typeof(TDbContext).FullName + "&&" + _context.Database.GetConnectionString() + "&&" + _context.ContextId.InstanceId;
            if (_unitOfWork.GetDbContext(dbContextKey) != null) //完全相同的直接返回
                return (TDbContext)_unitOfWork.GetDbContext(dbContextKey);

            await _unitOfWork.AddDbContextWithJudgeTransactionAsync(dbContextKey, _context);

            return _context;
        }
    }
}

/*=================================================================================================

=====================================Copyright Create Declare Start================================

Copyright © 2023 by OpenDeepSpace. All rights reserved.
Author: OpenDeepSpace	
CreateTime: 2023/6/19 21:15:57	
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
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.EntityFrameworkCore
{


    /// <summary>
    /// 上下文实体仓储接口 可以实现指定默认上下文
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IRepository<TEntity> where TEntity : class 
    {

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        TEntity Insert(TEntity entity);

        /// <summary>
        /// 异步添加
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<TEntity> InsertAsync(TEntity entity,CancellationToken cancellationToken=default(CancellationToken));


        /// <summary>
        /// 添加实体集合
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        void Insert(params TEntity[] entities);

        /// <summary>
        /// 异步添加实体集合
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="entities"></param>
        /// <returns></returns>
        Task InsertAsync(CancellationToken cancellationToken = default(CancellationToken), params TEntity[] entities);


        /// <summary>
        /// 添加实体集合
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        void Insert(IEnumerable<TEntity> entities);

        /// <summary>
        /// 异步添加实体集合
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken));


        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="updateProperties">指定需要更新的部分属性</param>
        TEntity Update(TEntity entity, Expression<Func<TEntity, object>>[] updateProperties = null);

        
    }

    /// <summary>
    /// 上下文实体仓储接口
    /// </summary>
    public interface IRepository<TDbContext,TEntity> : IRepository<TEntity> where TDbContext : DbContext where TEntity:class
    {

    }


}

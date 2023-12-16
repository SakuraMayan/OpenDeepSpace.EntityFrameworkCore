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
        TEntity Update(TEntity entity);

        /// <summary>
        /// 更新多个实体
        /// </summary>
        /// <param name="entities"></param>
        void Update(params TEntity[] entities);


        /// <summary>
        /// 更新实体集合
        /// </summary>
        /// <param name="entities"></param>
        void Update(IEnumerable<TEntity> entities);


        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity"></param>
        void Delete(TEntity entity);

        /// <summary>
        /// 删除多个实体
        /// </summary>
        /// <param name="entities"></param>
        void Delete(params TEntity[] entities);

        /// <summary>
        /// 删除实体集合
        /// </summary>
        /// <param name="entities"></param>
        void Delete(IEnumerable<TEntity> entities);




        //在efcore的CRUD操作之上 增加有关逻辑的可能的CRUD操作
        //TODO:增加忽略软删除的删除操作
        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="updateProperties">指定需要更新的部分属性</param>
        TEntity Update(TEntity entity, Expression<Func<TEntity, object>>[] updateProperties = null);

        /// <summary>
        /// 更新多个实体
        /// </summary>
        /// <param name="updateProperties">指定需要更新的部分属性</param>
        /// <param name="entities"></param>
        void Update(Expression<Func<TEntity, object>>[] updateProperties = null, params TEntity[] entities);


        /// <summary>
        /// 更新实体集合
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="updateProperties">指定需要更新的部分属性</param>
        void Update(IEnumerable<TEntity> entities, Expression<Func<TEntity, object>>[] updateProperties = null);


        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="ignoreSoftDelete">在存在软删除情况下 忽略软删除 如果设置为<see cref="true"/>表示直接硬删除 不考虑软删除</param>
        void Delete(TEntity entity, bool ignoreSoftDelete = false);

        /// <summary>
        /// 删除多个实体
        /// </summary>
        /// <param name="ignoreSoftDelete">在存在软删除情况下 忽略软删除 如果设置为<see cref="true"/>表示直接硬删除 不考虑软删除</param>
        /// <param name="entities"></param>
        void Delete(bool ignoreSoftDelete = false,params TEntity[] entities);

        /// <summary>
        /// 删除实体集合
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="ignoreSoftDelete">在存在软删除情况下 忽略软删除 如果设置为<see cref="true"/>表示直接硬删除 不考虑软删除</param>
        void Delete(IEnumerable<TEntity> entities, bool ignoreSoftDelete = false);

    }

    /// <summary>
    /// 上下文实体仓储接口
    /// </summary>
    public interface IRepository<TDbContext,TEntity> : IRepository<TEntity> where TDbContext : DbContext where TEntity:class
    {

    }


}

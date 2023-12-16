/*=================================================================================================

=====================================Copyright Create Declare Start================================

Copyright © 2023 by OpenDeepSpace. All rights reserved.
Author: OpenDeepSpace	
CreateTime: 2023/12/16 14:24:34	
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
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OpenDeepSpace.EntityFrameworkCore
{

    /// <summary>
    /// DbContext拓展
    /// </summary>
    public static class DbContextExtensions
    {
        /// <summary>
        /// 更新部分属性
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="entity"></param>
        /// <param name="updateProperties">指定需要更新的部分属性</param>
        /// <returns></returns>
        public static TEntity Update<TEntity>(this DbContext dbContext,TEntity entity, Expression<Func<TEntity, object>>[] updateProperties = null) where TEntity : class
        {
            //需要更新的字段
            if (updateProperties != null)
            {
                foreach (var property in updateProperties)
                {
                    dbContext.Entry(entity).Property(property).IsModified = true;
                }
                return entity;
            }
            else
            {
                //整体更新
                return dbContext.Update(entity).Entity;

            }

        }

        /// <summary>
        /// 更新部分属性
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="updateProperties">指定需要更新的部分属性</param>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static void UpdateRange<TEntity>(this DbContext dbContext, Expression<Func<TEntity, object>>[] updateProperties = null, params object[] entities) where TEntity : class
        {
            UpdateRange(dbContext, entities, updateProperties);

        }


        /// <summary>
        /// 更新部分属性
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbContext"></param>
        /// <param name="entities"></param>
        /// <param name="updateProperties">指定需要更新的部分属性</param>
        /// <returns></returns>
        public static void UpdateRange<TEntity>(this DbContext dbContext, IEnumerable<object> entities, Expression<Func<TEntity, object>>[] updateProperties = null) where TEntity : class
        {
            //需要更新的字段
            if (updateProperties != null)
            {
                foreach (TEntity entity in entities)
                {

                    foreach (var property in updateProperties)
                    {
                        dbContext.Entry(entity).Property(property).IsModified = true;
                    }
                }
            }
            else
            {
                //整体更新
                dbContext.UpdateRange(entities);
            }

        }
        
    }
}

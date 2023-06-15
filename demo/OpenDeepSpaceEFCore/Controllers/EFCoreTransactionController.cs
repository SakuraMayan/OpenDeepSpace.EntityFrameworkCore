using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OpenDeepSpaceEFCore.Controllers
{

    /// <summary>
    /// EFCore三种事务测试与理解
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EFCoreTransactionController : ControllerBase
    {


        public CustomDbContext CustomDbContext { get; set; }
        public CustomDbContext CustomDbContext2 { get; set; }

        public EFCoreTransactionController(CustomDbContext customDbContext, CustomDbContext customDbContext2)
        {
            CustomDbContext = customDbContext;
            CustomDbContext2 = customDbContext2;
        }




        /// <summary>
        /// EFCore默认自动事务 即调用SaveChanges之后默认会开启一个事务
        /// </summary>
        [HttpGet]
        public void EFCoreDefaultTrasaction()
        {
            /// <summary>
            ///     <para>
            ///         Gets or sets a value indicating whether or not a transaction will be created
            ///         automatically by <see cref="DbContext.SaveChanges()" /> if none of the
            ///         'BeginTransaction' or 'UseTransaction' methods have been called.
            ///         
            ///         获取或设置一个值来决定是否自动创建事务 | 如果BeginTransaction或UseTransaction方法没有被调用那么使用SaveChanges方法时将自动创建一个事务
            ///     </para>
            ///     <para>
            ///         Setting this value to <see langword="false" /> will also disable the <see cref="IExecutionStrategy" />
            ///         for <see cref="DbContext.SaveChanges()" />
            ///     </para>
            ///     <para>
            ///         The default value is <see langword="true" />, meaning that <see cref="DbContext.SaveChanges()" /> will always use a
            ///         transaction when saving changes.
            ///     </para>
            ///     <para>
            ///         Setting this value to <see langword="false" /> should only be done with caution since the database
            ///         could be left in a corrupted state if <see cref="DbContext.SaveChanges()" /> fails.
            ///     </para>
            /// </summary>
            /// <remarks>
            ///     See <see href="https://aka.ms/efcore-docs-transactions">Transactions in EF Core</see> for more information.
            /// </remarks>
            //CustomDbContext.Database.AutoTransactionsEnabled //默认SaveChanges调用将开启自动事务

           

            CustomDbContext.Add(new Role() { Id = Guid.NewGuid(), RoleName = $"角色{Guid.NewGuid()}" });

            //保存改变 将暂存 然后在由efcore默认的事务自动提交
            //这里如果CustomDbContext.Database.AutoTransactionsEnabled = false; 如果设置为false将不使用事务
            //但由于只有一个操作 不涉及事务
            CustomDbContext.SaveChanges();
        }

        [HttpGet]
        public void EFCoreDefaultTrasactionA()
        {//测试自动事务
            
            CustomDbContext.Add(new Role() { Id = Guid.NewGuid(), RoleName = $"角色{Guid.NewGuid()}" });
            //CustomDbContext.SaveChanges(); //在默认事务存在的情况下,每一个SaveChanges就是一个单独的独立的事务


            //以RoleName字段超长来模拟多个操作中出现异常事务回滚
            //CustomDbContext.Add(new Role() { Id = Guid.NewGuid(), RoleName = $"角色{Guid.NewGuid()}{Guid.NewGuid()}" });


            CustomDbContext.Add(new Role() { Id = Guid.NewGuid(), RoleName = $"角色{Guid.NewGuid()}" });

            //保存改变 然后在由efcore默认的事务自动提交
            CustomDbContext.SaveChanges();
        }

        [HttpGet]
        public void EFCoreDefaultTrasactionB()
        {//不启用自动事务

            /*
             * 
             以下不开启事务会有两种情况：
                1.第一条数据保存到数据库成功 :在SaveChanges不打断点暂停
                2.两条都不成功:如果在SaveChanges这里打个断点 隔一下时间在提交会发现第一条数据也不会成功插入 可能是因为CustomDbContext.Add数据由一定缓存时间，时间到了就没有数据了，因此不会写入到数据库中
            
            //关闭自动事务
            CustomDbContext.Database.AutoTransactionsEnabled = false;
            CustomDbContext.Add(new Role() { Id = Guid.NewGuid(), RoleName = $"角色{Guid.NewGuid()}" });

            //CustomDbContext.SaveChanges(); //  每调用一次SaveChange就是一次保存到数据库

            //这条由于RoleName超长将执行失败
            CustomDbContext.Add(new Role() { Id = Guid.NewGuid(), RoleName = $"角色{Guid.NewGuid()}{Guid.NewGuid()}" });

            CustomDbContext.SaveChanges();

             */

            //关闭自动事务
            CustomDbContext.Database.AutoTransactionsEnabled = false;
            CustomDbContext.Add(new Role() { Id = Guid.NewGuid(), RoleName = $"角色{Guid.NewGuid()}" });

            //CustomDbContext.SaveChanges(); //  每调用一次SaveChange就是一次保存到数据库

            //这条由于RoleName超长将执行失败
            CustomDbContext.Add(new Role() { Id = Guid.NewGuid(), RoleName = $"角色{Guid.NewGuid()}{Guid.NewGuid()}" });

            CustomDbContext.SaveChanges();
        }

        [HttpGet]
        public void EFCoreDbContextTransaction()
        {
            //使用手动管理事务
            using (CustomDbContext.Database.BeginTransaction())//开始事务 那么SaveChanges的默认事务将会失效 相当于托管给了手动管理 这里事务提交之后在执行SaveChanges 如果不开启新的事务那么又将回到efcore的自动事务
            {

                /*
                 CustomDbContext.Add(new Role() { Id = Guid.NewGuid(), RoleName = $"角色{Guid.NewGuid()}" });
                    CustomDbContext.Add(new Role() { Id = Guid.NewGuid(), RoleName = $"角色{Guid.NewGuid()}" });


                    //事务提交
                    CustomDbContext.Database.CommitTransaction();
                    如果像这段代码不调用SaveChanges 提交之后将不会保存到数据库 
                    那么SaveChanges可以理解为暂存数据到某个位置 然后事务提交时 真正写入数据库中
                 */

                /*
                 CustomDbContext.Add(new Role() { Id = Guid.NewGuid(), RoleName = $"角色{Guid.NewGuid()}" });

                    CustomDbContext.SaveChanges(); //暂存第一个
                    
                    CustomDbContext.Add(new Role() { Id = Guid.NewGuid(), RoleName = $"角色{Guid.NewGuid()}" });

                    CustomDbContext.SaveChanges();//暂存第二个

                    //事务提交
                    CustomDbContext.Database.CommitTransaction();
                 */

                /*
                 CustomDbContext.Add(new Role() { Id = Guid.NewGuid(), RoleName = $"角色{Guid.NewGuid()}" });

                    //CustomDbContext.SaveChanges();
                    
                    CustomDbContext.Add(new Role() { Id = Guid.NewGuid(), RoleName = $"角色{Guid.NewGuid()}" });

                    CustomDbContext.SaveChanges();//一次性暂存两个或多个 在最后调用

                    //事务提交
                    CustomDbContext.Database.CommitTransaction();
                 */

                /*
                 CustomDbContext.Add(new Role() { Id = Guid.NewGuid(), RoleName = $"角色{Guid.NewGuid()}" });

                    CustomDbContext.SaveChanges(); //这里的暂存将会由于第二条异常执行了事务回滚而回滚
                    
                    //模拟第二条数据出现异常
                    CustomDbContext.Add(new Role() { Id = Guid.NewGuid(), RoleName = $"角色{Guid.NewGuid()}{Guid.NewGuid()}" });

                    CustomDbContext.SaveChanges();//

                    //事务提交
                    CustomDbContext.Database.CommitTransaction();
                 */

                try
                {
                    CustomDbContext.Add(new Role() { Id = Guid.NewGuid(), RoleName = $"角色{Guid.NewGuid()}" });

                    CustomDbContext.SaveChanges();

                    //这里如果提交的事务 下面在提交需要开启一个新的事务才行
                    //如果没新开这个事务已经结束 又将回到efcore自动管理事务的状态 所以下面的SaveChanges会自动保存并提交事务
                    //CustomDbContext.Database.CommitTransaction();

                    //模拟第二条数据出现异常
                    CustomDbContext.Add(new Role() { Id = Guid.NewGuid(), RoleName = $"角色{Guid.NewGuid()}" });

                    CustomDbContext.SaveChanges();//一次性暂存两个或多个 在最后调用

                    //事务提交
                    CustomDbContext.Database.CommitTransaction();

                }
                catch (Exception ex)
                {
                    //出现异常事务回滚
                    CustomDbContext.Database.RollbackTransaction();
                }


            
            }

        
        }



    }
}

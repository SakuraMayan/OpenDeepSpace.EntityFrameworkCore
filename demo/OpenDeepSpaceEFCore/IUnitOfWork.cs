using Microsoft.EntityFrameworkCore;

namespace OpenDeepSpaceEFCore
{
    /// <summary>
    /// 工作单元 Scope范围内 每一次请求一个
    /// 如果要开启一个新的工作单元
    /// </summary>
    public interface IUnitOfWork
    {

        public Guid Id { get; set; }

        /// <summary>
        /// 保存改变:暂存数据 用于在提交事务之前
        /// 提供出接口 便于当不手动控制事务时 可自行调用 此时执行EFCORE默认事务 如果默认事务已关闭有没有手动控制 
        /// 那么保存改变后也会到数据库 只是事务不起作用即多个操作异常不会回滚 会出现一些成功一些失败的情况
        /// </summary>
        void SaveChanges();

        //开启事务在这里指定？ 这里指定就类似于显示开启事务了 还是在获取上下文的时候开启事务
        //void BeginTransaction();

        /// <summary>
        /// 提交事务
        /// </summary>
        void Commit();

        /// <summary>
        /// 回滚事务
        /// </summary>
        void RollBack();

        /// <summary>
        /// 向该工作单元中添加上下文
        /// </summary>
        /// <param name="dbContext"></param>
        void AddDbContext(DbContext dbContext);

        /// <summary>
        /// 查找上下文
        /// </summary>
        /// <param name="Id"></param>
        DbContext FindDbContext(Guid Id);//实际这里是上下文的全类型+连接字符串 由于存在类型和字符串的组合问题 可以考虑加入ContextId??如果上下文Id不同比如Transient那种情况好像不是很现实
    }
}

using Microsoft.EntityFrameworkCore;

namespace OpenDeepSpaceEFCore
{


    /*
     应该是每不同实体对应的仓储 都会产生一个新的上下文 而这个上下文一定要被某一个工作单元管理起来

工作单元应该被工作单元管理器管理起来 

先产生一个工作单元  然后获取上下文后加入到该工作单元  然后在仓储中加入刚刚产生的上下文

这个上下文他是既要在仓储中 又要在工作单元中 且这个上下文之间存在联系  多个不同上下文之间需要共享事务
     */


    /// <summary>
    /// Scoped的一次请求一个
    /// </summary>
    public interface IUnitOfWorkDbContextProvider
    {


        //获取上下文 获取到的上下文加入到工作单元中 考虑多个上下文共享事务的情况

        //如果存在工作单元的嵌套问题 开启新的工作单元即开启新的事务(这里就存在执行现在外面的工作单元是哪一个的问题 就存在当前工作单元是哪一个的问题) 即事务嵌套问题

        //必定存在外层工作单元 内层(当前工作单元)

        //如果存在读写分离 存在多个上下文共享连接 那么获取上下文如果采用注入的方式肯定不是共享上下文的
    }

    public interface IUnitOfWorkDbContextProvider<TDbContext> where TDbContext : DbContext
    { 
        public TDbContext GetDbContext();
    }
}

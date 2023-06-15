using Microsoft.EntityFrameworkCore;

namespace OpenDeepSpaceEFCore
{
    /*
     

默认事务只能单个上下文 不能多个上下文共享事务

手动管理事务 嵌套事务 多个上下文共享事务,通过传入相同的Options 工作单元

采用手动管理事务的方式来进行工作单元 所以这个SaveChanges可以不需要了

工作单元是手动管理事务的一种抽象

最后一定要SaveChanges一下 每一个SaveChanges也行 那么SaveChanges就不需要了

要考虑多个上下文如何共享事务


1.一个仓储就会产生一个上下文 一个工作单元可能会存在多个仓储执行 
2.一个仓储的每一次获取执行就对应一个上下文
3.我只需要把每一次获取到的上下文加入到当前工作单元中即可 那么这个工作单元应该是每次请求一个 如果需要新开启工作单元就是开启一个新的事务
4.那么就会有多个上下文如果这个多个上下文需要同一个事务即需要共享事务

仓储应该是每一次请求一个 仓储里面包含的数据库上下文应该也是每一次一个 

仓储每一次请求一个

工作单元的作用主要就是管理dbContext的事务的



返回一个空的虚假的工作单元

给工作单元 留一个本地 




字符串相同 类型相同的不同的实例  是一个 a DbContext1
字符串相同 类型不同  不是一个   a DbContext2

要考虑存在多个不同类型的上下文 使用一个字符串的情况 在一次工作单元中 能各自保证同一类型不同上下文事务的成功 
     */

    public class UnitOfWorkDbContextProvider<TDbContext>:IUnitOfWorkDbContextProvider<TDbContext> where TDbContext:DbContext
    {
        public TDbContext _dbContext;

        public IUnitOfWork _unitOfWork { get; set; }

        public UnitOfWorkDbContextProvider(TDbContext dbContext, IUnitOfWork unitOfWork)
        {
                _dbContext = dbContext;


            _unitOfWork = unitOfWork;


        }

         public TDbContext GetDbContext()
        {

            //如果已经添加了某个类型某个连接字符串的上下文就不再执行直接返回
            TDbContext dbContext = (TDbContext)_unitOfWork.FindDbContext(_unitOfWork.Id);
            if (_unitOfWork.FindDbContext(_unitOfWork.Id) != null)
                return dbContext;


            
            //把获取到的上下文加入到工作单元中管理 并隐式开启事务
            _unitOfWork.AddDbContext(_dbContext);

            return _dbContext;
        }

        //TODO:工作单元到这里就完成了 剩下的就是嵌套的实现 主要是通过工作单元管理器 产生一个嵌套的工作单元 最内层的工作单元为当前工作单元

        //TODO:多个不同实例共享上下文 如何实现
    }
}

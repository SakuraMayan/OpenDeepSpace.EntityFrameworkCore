using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenDeepSpace.EntityFrameworkCore;

namespace OpenDeepSpaceEntityFrameworkCore.Test.Controllers
{

    /// <summary>
    /// 数据库提供者测试
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DbContextProviderController : ControllerBase
    {
        public DbContextProviderController(CustomDbContext customDbContext, OtherDbContext otherDbContext, IDbContextTransactionManager transactionManager)
        {
            this.customDbContext = customDbContext;
            this.otherDbContext = otherDbContext;
            this.transactionManager = transactionManager;

            //这个加入位置可相应灵活的调整 可以在数据库生成的地方把数据库加入
            this.transactionManager.AddDbContextWithBeginTransaction(customDbContext);
            this.transactionManager.AddDbContextWithBeginTransaction(otherDbContext);

        }

        CustomDbContext customDbContext { get; set; }
        OtherDbContext otherDbContext { get; set; }

        IDbContextTransactionManager transactionManager { get; set; }

        [HttpGet]
        public void TestShareTransaction()
        {

            customDbContext.Add(new Role() { Id = Guid.NewGuid(), RoleName = $"角色{Guid.NewGuid()}" });
            customDbContext.SaveChanges();
            otherDbContext.Add(new Role() { Id = Guid.NewGuid(), RoleName = $"角色{Guid.NewGuid()}" });
            otherDbContext.SaveChanges();

            //提交事务
            this.transactionManager.CommitAsync(default,customDbContext);   
        }

    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OpenDeepSpaceEFCore.Controllers
{
    /// <summary>
    /// 工作单元测试控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UnitOfWorkController : ControllerBase
    {
        IUnitOfWork unitOfWork;
        //自定义上下文仓储
        IRepository<CustomDbContext,Role> roleRepo;

        public UnitOfWorkController(IUnitOfWork unitOfWork, IRepository<CustomDbContext, Role> roleRepo)
        {
            this.unitOfWork = unitOfWork;
            this.roleRepo = roleRepo;
        }

        [HttpGet]
        public void TestUnitOfWork()
        {

            

            roleRepo.InsertAsync(new Role() { Id = Guid.NewGuid(), RoleName = $"角色{Guid.NewGuid()}" });
            roleRepo.InsertAsync(new Role() { Id = Guid.NewGuid(), RoleName = $"一个异常的角色{Guid.NewGuid()}{Guid.NewGuid()}" });

            unitOfWork.Commit();

        }

    }
}

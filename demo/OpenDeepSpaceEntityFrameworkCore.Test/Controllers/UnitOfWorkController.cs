using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenDeepSpace.EntityFrameworkCore;
using OpenDeepSpace.NetCore.Hangfire;

namespace OpenDeepSpaceEntityFrameworkCore.Test.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UnitOfWorkController : ControllerBase
    {
        IUnitOfWork unitOfWork;
        //自定义上下文仓储
        IRepository<CustomDbContext, Role> roleRepo;
        IRepository<OtherDbContext, Role> otherRoleRepo;

        IBackgroundJobManager backgroundJobManager;

        public UnitOfWorkController(IUnitOfWork unitOfWork, IRepository<CustomDbContext, Role> roleRepo, IRepository<OtherDbContext, Role> otherRoleRepo, IBackgroundJobManager backgroundJobManager)
        {
            this.unitOfWork = unitOfWork;
            this.roleRepo = roleRepo;
            this.otherRoleRepo = otherRoleRepo;
            this.backgroundJobManager = backgroundJobManager;
        }

        [HttpGet]
        public void TestUnitOfWork()
        {



            roleRepo.Insert(new Role() { Id = Guid.NewGuid(), RoleName = $"角色{Guid.NewGuid()}" });
            //roleRepo.Insert(new Role() { Id = Guid.NewGuid(), RoleName = $"角色{Guid.NewGuid()}" });
            roleRepo.Insert(new Role() { Id = Guid.NewGuid(), RoleName = $"一个异常的角色{Guid.NewGuid()}{Guid.NewGuid()}" });

            unitOfWork.Commit();

        }

        [HttpGet]
        public async Task TestUnitOfWorkShareSqlConnection()
        {



            roleRepo.Insert(new Role() { Id = Guid.NewGuid(), RoleName = $"角色{Guid.NewGuid()}" });
            roleRepo.Insert(new Role() { Id = Guid.NewGuid(), RoleName = $"角色{Guid.NewGuid()}" });
            //otherRoleRepo.Insert(new Role() { Id = Guid.NewGuid(), RoleName = $"一个异常的角色{Guid.NewGuid()}{Guid.NewGuid()}" });

            //触发Job执行
            //await backgroundJobManager.EnqueueAsync(new RoleJobArgs() { });
            backgroundJobManager.EnqueueAsync(new RoleJobArgs() { });

            unitOfWork.Commit();

        }
    }
}

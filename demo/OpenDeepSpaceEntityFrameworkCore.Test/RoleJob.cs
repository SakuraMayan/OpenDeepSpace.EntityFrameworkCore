using OpenDeepSpace.EntityFrameworkCore;
using OpenDeepSpace.NetCore.Hangfire;

namespace OpenDeepSpaceEntityFrameworkCore.Test
{
    /// <summary>
    /// 在后台进行角色添加
    /// </summary>
    public class RoleJob : AsyncBackgroundJob<RoleJobArgs>
    {

        private IUnitOfWork _unitOfWork;
        private IRepository<CustomDbContext, Role> _repo;
        private IRepository<OtherDbContext, Role> _otherRepo;

        public RoleJob(IRepository<CustomDbContext, Role> repo, IRepository<OtherDbContext, Role> otherRepo, IUnitOfWork unitOfWork)
        {
            _repo = repo;
            _otherRepo = otherRepo;
            _unitOfWork = unitOfWork;
        }

        public override async Task ExecuteAsync(RoleJobArgs args)
        {
            _repo.Insert(new Role() { Id = Guid.NewGuid(), RoleName = $"角色{Guid.NewGuid()}" });
            //_otherRepo.Insert(new Role() { Id = Guid.NewGuid(), RoleName = $"角色{Guid.NewGuid()}" });
            _otherRepo.Insert(new Role() { Id = Guid.NewGuid(), RoleName = $"这是一个异常的角色{Guid.NewGuid()}{Guid.NewGuid()}" });

            _unitOfWork.Commit();//提交事务

        }
    }

    public class RoleJobArgs 
    {
    }
}

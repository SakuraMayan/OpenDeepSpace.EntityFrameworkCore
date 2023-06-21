using Microsoft.Extensions.DependencyInjection;
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
        private IServiceScopeFactory _scopeFactory;

        public RoleJob(IRepository<CustomDbContext, Role> repo, IRepository<OtherDbContext, Role> otherRepo, IUnitOfWork unitOfWork, IServiceScopeFactory scopeFactory)
        {
            _repo = repo;
            _otherRepo = otherRepo;
            _unitOfWork = unitOfWork;

            //不开启事务
            _unitOfWork.Initialize(new UnitOfWorkOptions() { IsTransactional = false });
            _scopeFactory = scopeFactory;
        }

        public override async Task ExecuteAsync(RoleJobArgs args)
        {
            _repo.Insert(new Role() { Id = Guid.NewGuid(), RoleName = $"角色{Guid.NewGuid()}" });
            //_otherRepo.Insert(new Role() { Id = Guid.NewGuid(), RoleName = $"角色{Guid.NewGuid()}" });
            _otherRepo.Insert(new Role() { Id = Guid.NewGuid(), RoleName = $"这是一个异常的角色{Guid.NewGuid()}{Guid.NewGuid()}" });

            _unitOfWork.Commit();//提交事务

            //尝试开启一个新的工作单元完成互相不影响的操作 使用Scope 所有对象都要重新获取才能保证一致性
            using (var scopeUow = _scopeFactory.CreateScope())
            {
                //全部采用新的范围获取
                _unitOfWork=scopeUow.ServiceProvider.GetRequiredService<IUnitOfWork>();
                _repo = scopeUow.ServiceProvider.GetRequiredService<IRepository<CustomDbContext, Role>>();
                _otherRepo = scopeUow.ServiceProvider.GetRequiredService<IRepository<OtherDbContext, Role>>();

                _repo.Insert(new Role() { Id = Guid.NewGuid(), RoleName = $"角色{Guid.NewGuid()}" });
                //_otherRepo.Insert(new Role() { Id = Guid.NewGuid(), RoleName = $"角色{Guid.NewGuid()}" });
                _otherRepo.Insert(new Role() { Id = Guid.NewGuid(), RoleName = $"这是一个异常的角色{Guid.NewGuid()}{Guid.NewGuid()}" });

                _unitOfWork.Commit();//提交事务

            }


        }
    }

    public class RoleJobArgs 
    {
    }
}

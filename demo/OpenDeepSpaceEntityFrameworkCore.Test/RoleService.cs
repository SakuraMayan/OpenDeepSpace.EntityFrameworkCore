using OpenDeepSpace.EntityFrameworkCore;

namespace OpenDeepSpaceEntityFrameworkCore.Test
{

    /// <summary>
    /// 
    /// </summary>
    public class RoleService : IRoleService
    {
        IUnitOfWork unitOfWork;
        IRepository<CustomDbContext, Role> roleRepo;
        IRepository<OtherDbContext, Role> otherRoleRepo;

        public RoleService(IUnitOfWork unitOfWork, IRepository<CustomDbContext, Role> roleRepo, IRepository<OtherDbContext, Role> otherRoleRepo)
        {
            this.unitOfWork = unitOfWork;
            this.roleRepo = roleRepo;
            this.otherRoleRepo = otherRoleRepo;
        }

        public Role AddRole(Role role)
        {
            roleRepo.Insert(new Role() { Id = Guid.NewGuid(), RoleName = $"角色{Guid.NewGuid()}" });
            role = otherRoleRepo.Insert(new Role() { Id = Guid.NewGuid(), RoleName = $"一个异常的角色{Guid.NewGuid()}{Guid.NewGuid()}" }); 
            //role = otherRoleRepo.Insert(new Role() { Id = Guid.NewGuid(), RoleName = $"角色{Guid.NewGuid()}" }); 

            return role;    
        }
    }
}

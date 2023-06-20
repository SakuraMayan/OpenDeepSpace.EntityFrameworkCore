using Microsoft.EntityFrameworkCore;

namespace OpenDeepSpaceEntityFrameworkCore.Test
{
    /// <summary>
    /// 自定义上下文
    /// </summary>
    public class OtherDbContext : DbContext
    {
        public DbSet<Role> Role { get; set; }



        public OtherDbContext(DbContextOptions<OtherDbContext> options) : base(options)
        {
        }
    }
}

using Microsoft.EntityFrameworkCore;

namespace OpenDeepSpaceEFCore
{


    /// <summary>
    /// 自定义上下文
    /// </summary>
    public class CustomDbContext:DbContext
    {
        public DbSet<Role> Role { get; set; }


        public CustomDbContext(DbContextOptions<CustomDbContext> options) :base(options)
        {
                
        }
    }
}

using Microsoft.EntityFrameworkCore;

namespace OpenDeepSpaceEFCore
{

    /*
     
     */
    public class OtherDbContext : DbContext
    {

        public DbSet<Role> Role { get; set; }

        public Guid Id { get; set; }


        public OtherDbContext(DbContextOptions<OtherDbContext> options) : base(options)
        {
            Id = Guid.NewGuid();
        }
    }
}

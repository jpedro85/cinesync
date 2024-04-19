using Microsoft.AspNetCore.Identity;

namespace CineSync.Data
{
    public class DbRolesInitializer : DbInitializer<IdentityRole>
    {
        public DbRolesInitializer(ApplicationDbContext context) : base(context) { }
        public override bool isDuplicate(IdentityRole entity)
        {
            return context.Set<IdentityRole>().Any(user => user.NormalizedName == entity.NormalizedName);
        }
    }
}

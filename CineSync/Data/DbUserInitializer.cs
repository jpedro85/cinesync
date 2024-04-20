namespace CineSync.Data
{
    public class DbUserInitializer : DbInitializer<ApplicationUser>
    {
        public DbUserInitializer(ApplicationDbContext context) : base(context) { }
        public override bool isDuplicate( ApplicationUser entity )
        {
             return context.Set<ApplicationUser>().Any( user => user.UserName == entity.UserName);
        }
    }
}

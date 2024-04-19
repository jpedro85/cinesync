using CineSync.Data;
using Microsoft.AspNetCore.Identity;
using static System.Formats.Asn1.AsnWriter;

namespace CineSync
{
    public class ServerConfigurationFacade
    {
        private WebApplication app;

        public ServerConfigurationFacade(WebApplication app) 
        { 
            this.app = app;
        }

        public void Config()
        {
            InitializeDb();
        }

        private void InitializeDb ()
        {
            var services = app.Services.CreateScope().ServiceProvider;
            var dbContext = services.GetRequiredService<ApplicationDbContext>();

            InitializeDbSetUser(dbContext);

            InitializeDbSetRoles(dbContext);
        }

        private void InitializeDbSetRoles (ApplicationDbContext dbContext)
        {
            var roleinitializer = new DbRolesInitializer(dbContext);
            roleinitializer
                .WithEntity(new IdentityRole() { Name = "vititor", NormalizedName = "visitor" })
                .WithEntity(new IdentityRole() { Name = "user", NormalizedName = "user" })
                .WithEntity(new IdentityRole() { Name = "moderator", NormalizedName = "moderator" })
                .WithEntity(new IdentityRole() { Name = "admin", NormalizedName = "admin" })
                .WithEntity(new IdentityRole() { Name = "super admin", NormalizedName = "super admin" })
                .Initialize();
        }

        private void InitializeDbSetUser(ApplicationDbContext dbContext)
        {
            var userInitializer = new DbUserInitializer(dbContext);
            userInitializer
                .WithEntity(new ApplicationUser() { UserName = "Ghost", Id = "0" })
                .Initialize();

        }
    }
}

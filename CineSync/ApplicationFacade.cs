using CineSync.Components;
using CineSync.Components.Account;
using CineSync.Data;
using CineSync.Data.Models;
using CineSync.DbManagers;
using CineSync.Middleware;
using CineSync.Controllers;
using CineSync.Controllers.MovieEndpoint;
using CineSync.Core;
using CineSync.Core.Adapters.ApiAdapters;
using CineSync.Core.Logger;
using CineSync.Core.Repository;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CineSync.Services;

namespace CineSync
{
    public class ApplicationFacade
    {
        private WebApplicationBuilder builder;

        private string formattedDate;

        public ApplicationFacade(WebApplicationBuilder builder, string LogdateFormat)
        {
            this.builder = builder;
            formattedDate = DateTime.Now.ToString(LogdateFormat);
        }

        public WebApplication ConfigApplication()
        {
            ConfigureServices(builder.Services);
            WebApplication app = builder.Build();
            ConfigureApp(app);
            InitializeDb(app);

            return app;
        }

        private void ConfigureServices(IServiceCollection services)
        {
            AddRazorComponents(services);
            AddLogger(services);
            AddAuthenticationServices(services);
            AddDatabaseServices(services);
            AddAuthorizationPolicies(services);
            AddAdditionalServices(services);
        }

        private void AddRazorComponents(IServiceCollection services)
        {
            services.AddRazorComponents()
                .AddInteractiveServerComponents()
                .AddInteractiveWebAssemblyComponents();

            services.AddScoped<IdentityUserAccessor>();
            services.AddScoped<IdentityRedirectManager>();
            services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();
            services.AddControllers();
        }
        private void AddLogger(IServiceCollection services)
        {
            services.AddSingleton<ILoggerStrategy>(provider =>
                new LoggerBuilder()
                    .UseConsoleLogging()
                    .AddTimeStamp()
                    .AddType()
                    .UseDebugLogging()
                    .AddTimeStamp()
                    .UseTraceDebugging()
                    .UseFileLogging($"./Logs/log_{formattedDate}.log")
                    .AsPlainText()
                    .AddTimeStamp()
                    .AddType()
                    .Build());
        }

        private void AddAuthenticationServices(IServiceCollection services)
        {
            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = IdentityConstants.ApplicationScheme;
                    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
                })
                .AddIdentityCookies();

            services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();
        }

        private void AddDatabaseServices(IServiceCollection services)
        {
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connectionString));
            services.AddDatabaseDeveloperPageExceptionFilter();
        }

        private void AddAuthorizationPolicies(IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireUser", policy => { policy.RequireRole("user"); });
                options.AddPolicy("RequireModerator", policy => { policy.RequireRole("moderator"); });
                options.AddPolicy("RequireAdmin", policy => { policy.RequireRole("admin"); });
                options.AddPolicy("RequireSuperAdmin", policy => { policy.RequireRole("super admin"); });
            });
            services.AddCascadingAuthenticationState();
        }

        private void AddAdditionalServices(IServiceCollection services)
        {

            var types = new[] {
                    typeof(Movie),
                    typeof(CollectionsMovies),
                    typeof(MovieCollection),
                    typeof(Genre),
                    typeof(MoviesGenres),
                    typeof(Discussion),
                    typeof(Comment),
                    typeof(CommentAttachment),
                    typeof(Notification),
                    typeof(UsersNotifications)
            };

            services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
            services.AddHttpClient<ApiService>();
            services.AddScoped<MovieController>();
            services.AddScoped<MovieDetailsAdapter>();
            services.AddSingleton<IFactory>(sp => new Factory(types));
            services.AddScoped<IUnitOfWork, UnitOfWork<ApplicationDbContext>>();
            services.AddScoped<IUnitOfWorkAsync, UnitOfWorkAsync<ApplicationDbContext>>();
            services.AddScoped<MovieController>();
            services.AddScoped<MovieManager>();
            services.AddScoped<UserRoleManager<ApplicationUser>>();
            services.AddScoped<MovieCollectionsManager>();
            services.AddScoped<CommentManager>();
            services.AddScoped<DiscussionManager>();
            services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5145") });
            // INFO: Solves the issue of supposedly loop of object when saving the parsed json to the database
            services.AddControllers().AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
                });

            services.AddSingleton<NavBarEvents>();
            services.AddSingleton<LayoutService>();
           
        }

        private void InitializeDb(WebApplication app)
        {
            var services = app.Services.CreateScope().ServiceProvider;
            var dbContext = services.GetRequiredService<ApplicationDbContext>();

            InitializeDbSetUser(dbContext);
            InitializeDbSetRoles(dbContext);
        }

        private void ConfigureApp(WebApplication app)
        {
            ConfigureHTTP(app);

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseMiddleware<ErrorLoggingMiddleware>();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseAntiforgery();
            //app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            app.MapControllers();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode()
                .AddInteractiveWebAssemblyRenderMode()
                .AddAdditionalAssemblies(typeof(CineSync.Client._Imports).Assembly);

            app.MapAdditionalIdentityEndpoints();
        }

        private void ConfigureHTTP(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error", createScopeForErrors: true);
                app.UseHsts();
            }
        }

        private void InitializeDbSetRoles(ApplicationDbContext dbContext)
        {
            var roleinitializer = new DbRolesInitializer(dbContext);
            roleinitializer
                .WithEntity(new IdentityRole() { Name = "user", NormalizedName = "user" })
                .WithEntity(new IdentityRole() { Name = "moderator", NormalizedName = "moderator" })
                .WithEntity(new IdentityRole() { Name = "admin", NormalizedName = "admin" })
                .WithEntity(new IdentityRole() { Name = "super_admin", NormalizedName = "super_admin" })
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

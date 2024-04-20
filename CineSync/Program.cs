using CineSync;
using CineSync.Client.Pages;
using CineSync.Components;
using CineSync.Components.Account;
using CineSync.Data;
using CineSync.Middleware;
using CineSync.Controllers;
using CineSync.Controllers.Movie;
using CineSync.Utils.Logger;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using dotenv.net;

DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);
string formattedDate = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();
builder.Services.AddControllers();
builder.Services.AddHttpClient<ApiService>();
builder.Services.AddScoped<MovieController>();
builder.Services.AddSingleton<ILoggerStrategy>(provider =>
        new LoggerBuilder()
            .UseConsoleLogging()
            .AddTimeStamp()
            .AddType()
            .UseDebugLogging()
            .AddTimeStamp()
            .UseTraceDebugging()
            .UseFileLogging($"./Logs/log_{formattedDate}.log")
            .AddTimeStamp()
            .AddType()
            .Build());

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>( options => options.SignIn.RequireConfirmedAccount = false )
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("RequireAdmin", policy =>
	{
		policy.RequireRole("Admin");
	});
});


builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

var app = builder.Build();


// Configure the HTTP request pipeline.
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


ServerConfigurationFacade serverConfigurationFacade = new ServerConfigurationFacade(app);
serverConfigurationFacade.Config();


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseMiddleware<ErrorLoggingMiddleware>();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(CineSync.Client._Imports).Assembly);

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();

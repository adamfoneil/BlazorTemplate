using Application;
using Application.Client;
using Application.Components.Account;
using Application.Extensions;
using Application.Models;
using ClientHelpers;
using Coravel;
using Domain;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Radzen;
using Serilog;
using Serilog.Components;
using Service;

Log.Logger = new LoggerConfiguration()
	.WriteTo.Console()
	.CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Logging.SetMinimumLevel(LogLevel.Debug);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.Configure<SerilogOptions>(builder.Configuration.GetSection("SerilogOptions"));
builder.Services.Configure<ConnectionStrings>(builder.Configuration.GetSection("ConnectionStrings"));

builder.Services.AddSerilog((services, config) => config
	.ReadFrom.Configuration(builder.Configuration)
	.ReadFrom.Services(services)
	.Enrich.FromLogContext()
	.WriteTo.Console(Extensions.CustomConsoleOutput)
	.WriteTo.SqlServerCustomConfig(builder.Configuration));

builder.Services.AddSerilogSqlServerCleanup(builder.Configuration);

builder.Services.AddScheduler();

builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents()
	.AddInteractiveWebAssemblyComponents();

builder.Services.AddRadzenComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();
builder.Services.AddSingleton<BaseUrlProvider>();
builder.Services.AddScoped<ApiEventHandler>(); // this doesn't seem to work from the backend
builder.Services.AddScoped<ApiClient>();
builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, ApplicationUserClaimsPrincipalFactory>();
builder.Services.AddScoped<CircuitHandler, LoggingCircuitHandler>();

builder.Services.AddAuthentication(options =>
	{
		options.DefaultScheme = IdentityConstants.ApplicationScheme;
		options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
	})
	.AddIdentityCookies();

builder.Services.DisableApiRedirectToLogin();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
	options.UseSqlServer(connectionString);
	//options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
});
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
	.AddRoles<IdentityRole>()
	.AddRoleManager<RoleManager<IdentityRole>>()
	.AddEntityFrameworkStores<ApplicationDbContext>()	
	.AddSignInManager()	
	.AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
builder.Services.AddHttpClient<CookieHandler>(ApiClient.Name, (sp, client) => sp.GetRequiredService<BaseUrlProvider>().BaseUrl);

builder.Services.MigrateDatabase<ApplicationDbContext>();

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
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.Services.UseScheduler(scheduler =>
{
	scheduler.Schedule<SqlServerCleanup>().DailyAtHour(2);
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.UseSerilogUserName();
app.UseSerilogRequestLogging();

app.MapRazorComponents<Application.Components.App>()
	.AddInteractiveServerRenderMode()
	.AddInteractiveWebAssemblyRenderMode()
	.AddAdditionalAssemblies(typeof(ApiClient).Assembly);

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

var apiGroup = app.MapGroup("/api").RequireAuthorization();

new DbSetApiHandler<Widget>("/widgets").Map(apiGroup);	

app.Run();

#region CookieAuthenticationStateProvider
/*
 * not using this because I'm using Application.Client.PersistentAuthenticationStateProvider.
 * this would have been used with AppCookieAuthenticationStateProvider.
apiGroup.MapGet("/userinfo", async (HttpContext context) =>
{
	var sp = context.RequestServices;
	var userManager = sp.GetRequiredService<UserManager<ApplicationUser>>();
	var user = await userManager.GetUserAsync(context.User);
	if (user is null) return Results.NotFound();
	return Results.Ok(UserInfo.FromApplicationUser(user));
});
*/
#endregion

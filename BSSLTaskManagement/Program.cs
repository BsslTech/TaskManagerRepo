using BSSLTaskManagement.Services;
using BSSLTaskManagement.ServicesInterfaces;
using Microsoft.EntityFrameworkCore;
using TaskManagement;
using static TaskManagement.IdentityLib;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("taskConString") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<TaskDbContext>(options =>
    options.UseSqlServer(connectionString));
const int commandTimeoutInSeconds = 100;


builder.Services.AddDbContext<TaskDbContext>(opt =>
    opt.UseSqlServer(connectionString,
    b => {
        b.MigrationsAssembly("BSSLTaskManagement");
        b.CommandTimeout(commandTimeoutInSeconds);
    }));
builder.Services.AddDefaultIdentity<TaskIdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<TaskIdentityRole>()
    .AddEntityFrameworkStores<TaskDbContext>();



builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.ReturnUrlParameter = "returnUrl";

    // IMPORTANT
    options.SlidingExpiration = true;
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.AddScoped<ISystemSerivces, SystemSerivces>();
builder.Services.AddScoped<IMainMenuSetupServices, MainMenuSetupServices>();
builder.Services.AddScoped<IUserManagementServices, UserManagementServices>();
builder.Services.AddScoped<IConstituencyService, ConstituencyService>();
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/"); // protect everything
    options.Conventions.AllowAnonymousToFolder("/Identity"); // allow login
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Redirect root to Index (do not redirect to Login)
app.MapGet("/", context =>
{
    context.Response.Redirect("/Index");
    return Task.CompletedTask;
});
app.MapRazorPages();
app.MapControllers();
app.Run();

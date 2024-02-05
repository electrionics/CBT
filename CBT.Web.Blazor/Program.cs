using Syncfusion.Blazor;
using Syncfusion.Blazor.Popups;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Components.Authorization;

using CBT.Web.Blazor.Data.Identity;
using CBT.Web.Blazor.Services;
using CBT.Web.Blazor.Hubs;
using CBT.Web.Blazor.Background;
using CBT.Web.Blazor.Services.Authentication;
using Microsoft.AspNetCore.CookiePolicy;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("CBTIdentityDataContextConnection") ?? throw new InvalidOperationException("Connection string 'CBTIdentityDataContextConnection' not found.");

builder.Services.AddDbContext<CBTIdentityDataContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<CBTIdentityDataContext>().AddDefaultTokenProviders();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("https://localhost:7040", "http://localhost:51979")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials());
});

builder.Services.AddAuthentication();

// JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

builder.Services.AddAuthorization();

builder.Services.AddSignalR();

builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSyncfusionBlazor();

builder.Services.AddSingleton<AutomaticThoughtsService>();
builder.Services.AddSingleton<PsychologistReviewService>(); 
builder.Services.AddScoped<SfDialogService>();
builder.Services.AddScoped<UserManager<User>>();

builder.Services.AddHostedService<UserNotificationService>();

//builder.Services.AddScoped<JwtProvider>();
//builder.Services.ConfigureOptions<JwtOptionsSetup>();
//builder.Services.ConfigureOptions<JwtBearerOptionsSetup>();

//builder.Services.AddScoped<CustomAuthenticationStateProvider>();
//builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<CustomAuthenticationStateProvider>());

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.None;
    options.HttpOnly = HttpOnlyPolicy.None;
    options.Secure = CookieSecurePolicy.None;
    options.CheckConsentNeeded = _ => false;
});

//builder.Services.AddScoped<AuthenticationStateProvider, CBTAuthenticationStateProvider>();

var app = builder.Build();
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzA4NTYxNUAzMjM0MmUzMDJlMzBpeGxzb082cEZHalkwdGl1UE5aU25lcGI5R1RqYW5HRG1WTks1anFjN1JRPQ==");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapHub<NotificationHub>("notifications");
app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

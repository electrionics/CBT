using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.CookiePolicy;

using Syncfusion.Blazor;
using Syncfusion.Blazor.Popups;
using Serilog;
using FluentValidation;

using CBT.Web.Blazor;
using CBT.Web.Blazor.Services;
using CBT.Web.Blazor.Hubs;
using CBT.Web.Blazor.Background;
using CBT.Web.Blazor.Data;
using CBT.Web.Blazor.Data.Identity;
using CBT.Web.Blazor.Data.Model.Validators;
using CBT.Web.Blazor.Data.Model.Identity;

var builder = WebApplication.CreateBuilder(args);
var databaseConfig = builder.Configuration.GetSection("Database").Get<DatabaseConfig>();

builder.Services.AddSingleton(databaseConfig!);

builder.Services.AddDbContext<CBTIdentityDataContext>(options =>
{
    options.UseSqlServer(databaseConfig?.SingleConnectionString);
});
builder.Services.AddDbContext<CBTDataContext>((options) =>
{
    options.UseSqlServer(databaseConfig?.SingleConnectionString);
});
builder.Services.AddDbContext<CBTDataContextMARS>((options) =>
{
    options.UseSqlServer(databaseConfig?.SingleConnectionStringMARS);
});


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

builder.Services.Configure<IdentityOptions>(options =>
{
    // Default Password settings.
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 0;
});


builder.Services.AddSignalR((options) =>
{
    options.EnableDetailedErrors = true;
});

builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSyncfusionBlazor();

builder.Services.AddScoped<AutomaticThoughtsService>();
builder.Services.AddScoped<PsychologistReviewService>();
builder.Services.AddScoped<CognitiveErrorsService>();
builder.Services.AddScoped<EmotionsService>();
builder.Services.AddScoped<PeopleService>(); 
builder.Services.AddScoped<NotificationsService>(); 
builder.Services.AddScoped<SfDialogService>();
builder.Services.AddScoped<UserManager<User>>();

builder.Services.AddScoped<IValidator<LoginModel>, LoginModelValidator>();
builder.Services.AddScoped<IValidator<RegisterModel>, RegisterModelValidator>();

builder.Services.AddHostedService<UserNotificationBackgroundService>();

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

var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);
builder.Host.UseSerilog(logger);

//builder.Services.AddScoped<AuthenticationStateProvider, CBTAuthenticationStateProvider>();

var app = builder.Build();
// old Ngo9BigBOggjHTQxAR8/V1NAaF5cWWJCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdnWXxednRRRmVcVkJ2V0I=
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NCaF5cXmZCeUx/WmFZfVpgdVdMZVtbR3FPIiBoS35RckVkWX1fcnFSRWdVUEB1");

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
app.UseSerilogRequestLogging();
app.UseRouting();
app.UseAuthorization();

app.MapHub<NotificationHub>("notifications");
app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

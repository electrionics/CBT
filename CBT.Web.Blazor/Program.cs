using Serilog;

using CBT.SharedComponents.Blazor;

using CBT.Web.Blazor.Hubs;
using CBT.Web.Blazor.Background;
using CBT.Shared.Blazor;

var builder = WebApplication.CreateBuilder(args);
var databaseConfig = builder.Configuration.GetSection("Database").Get<DatabaseConfig>();

builder.Services.Extend(databaseConfig!);

builder.Services.AddHostedService<UserNotificationBackgroundService>();

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

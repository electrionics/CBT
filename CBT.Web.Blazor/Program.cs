using Serilog;

using CBT.SharedComponents.Blazor;

using CBT.Web.Blazor;
using CBT.Web.Blazor.Hubs;
using CBT.Web.Blazor.Background;

var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);
builder.Host.UseSerilog(logger);

var builderServices = builder.Services;

try
{
    #region Web

    builderServices.AddCors(options =>
    {
        options.AddPolicy("AllowSpecificOrigin",
            builder => builder.WithOrigins("https://localhost:7040", "http://localhost:51979")
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials());
    });

    builderServices.AddAntiforgery();
    builderServices.AddHostedService<UserNotificationBackgroundService>();

    #endregion

    builderServices
        .WithConfigurations(builder.Configuration)
        .WithDatabase(builder.Configuration)
        .WithServices()
        .WithValidators()
        .WithIdentity()
        .WithCommon()
        .WithWeb();

    //builder.Services.AddScoped<AuthenticationStateProvider, CBTAuthenticationStateProvider>();

    var app = builder.Build();
    // old Ngo9BigBOggjHTQxAR8/V1NAaF5cWWJCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdnWXxednRRRmVcVkJ2V0I=
    // 
    Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NBaF5cXmZCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdnWXtfcHRRR2deV0JyW0c=");

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
    app.UseAntiforgery();

    app.MapHub<NotificationHub>("notifications");
    app.MapControllers();
    app.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode();

    app.Run();
}
catch(Exception ex)
{
    logger.Fatal("Can't start application" + ex.ToString(), ex);
}

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Components.Authorization;

using CBT.SharedComponents.Blazor;
using CBT.MAUI.Blazor.Infrastructure;

namespace CBT.MAUI.Blazor
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            #region Default Blazor Hybrid

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            
            builder.Services.AddMauiBlazorWebView();
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            #endregion

            builder.Configuration.AddJsonFile("appsettings.desktop.json");
            var databaseConfig = builder.Configuration.GetSection("Database").Get<DatabaseConfig>();
            var builderServices = builder.Services;
            builderServices
                .WithDatabase(databaseConfig!)
                .WithServices()
                .WithValidators()
                .WithIdentity()
                .WithCommon();

            #region Error Handling

            builderServices.AddExceptionHandler<CustomExceptionHandler>();

            #endregion

            builderServices.AddAuthorizationCore();
            builderServices.AddScoped<AuthenticationStateProvider, CurrentThreadUserAuthenticationStateProvider>();

            return builder.Build();
        }
    }
}

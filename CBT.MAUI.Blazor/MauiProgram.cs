using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Components.Authorization;

using CBT.SharedComponents.Blazor;
using CBT.SharedComponents.Blazor.Common;
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

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NCaF5cXmZCeUx/WmFZfVpgdVdMZVtbR3FPIiBoS35RckVkWX1fcnFSRWdVUEB1");

            builder.Configuration.AddJsonFile("appsettings.desktop.json");

            var builderServices = builder.Services;

            builderServices
                .WithConfigurations(builder.Configuration)
                .WithDatabase(builder.Configuration)
                .WithServices()
                .WithValidators()
                .WithIdentity()
                .WithCommon();

            #region Error Handling

            builderServices.AddExceptionHandler<CustomExceptionHandler>();

            #endregion

            builderServices.AddAuthorizationCore();

            return builder.Build();
        }
    }
}

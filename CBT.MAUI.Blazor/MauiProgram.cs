using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using CBT.SharedComponents.Blazor;

namespace CBT.MAUI.Blazor
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();


            #region From Web
            builder.Configuration.AddJsonFile("appsettings.desktop.json");
            var databaseConfig = builder.Configuration.GetSection("Database").Get<DatabaseConfig>();
            builder.Services.Extend(databaseConfig!);

            #endregion
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();

            
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}

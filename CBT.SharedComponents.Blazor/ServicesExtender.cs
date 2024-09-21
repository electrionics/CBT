using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using FluentValidation;

using Syncfusion.Blazor;
using Syncfusion.Blazor.Popups;

using CBT.Domain;
using CBT.Domain.Identity;
using CBT.Logic.Services;
using CBT.SharedComponents.Blazor.Common;
using CBT.SharedComponents.Blazor.Model.Identity;
using CBT.SharedComponents.Blazor.Model.Validators;
using CBT.SharedComponents.Blazor.Services;

namespace CBT.SharedComponents.Blazor
{
    public static class ServicesExtender
    {
        public static IServiceCollection WithConfigurations(this IServiceCollection builderServices, ConfigurationManager builderConfiguration)
        {
            var databaseConfig = builderConfiguration.GetSection("Database").Get<DatabaseConfig>();
            var apiConfig = builderConfiguration.GetSection("Api").Get<ApiConfig>();

            builderServices.AddSingleton(databaseConfig!);
            builderServices.AddSingleton(apiConfig!);

            return builderServices;
        }

        public static IServiceCollection WithDatabase(this IServiceCollection builderServices, ConfigurationManager builderConfiguration)
        {
            var databaseConfig = builderConfiguration.GetSection("Database").Get<DatabaseConfig>();

            builderServices.AddDbContext<CBTIdentityDataContext>(options =>
            {
                options.UseSqlServer(databaseConfig?.SingleConnectionString);
            }, ServiceLifetime.Transient);
            builderServices.AddDbContext<CBTDataContext>((options) =>
            {
                options.UseSqlServer(databaseConfig?.SingleConnectionString);
            }, ServiceLifetime.Transient);
            builderServices.AddDbContext<CBTDataContextMARS>((options) =>
            {
                options.UseSqlServer(databaseConfig?.SingleConnectionStringMARS);
            }, ServiceLifetime.Transient);

            return builderServices;
        }

        public static IServiceCollection WithServices(this IServiceCollection builderServices)
        {
            builderServices.AddScoped<AutomaticThoughtsService>();
            builderServices.AddScoped<PeopleService>();
            builderServices.AddScoped<NotificationsService>();
            builderServices.AddScoped<SfDialogService>();
            builderServices.AddScoped<UserManager<User>>();

            builderServices.AddScoped<DiariesFacade>();
            builderServices.AddScoped<PsychologistReviewFacade>();
            builderServices.AddScoped<CognitiveErrorsFacade>();
            builderServices.AddScoped<EmotionsFacade>();

            builderServices.AddScoped<IEmailSender, EmailService>();

            return builderServices;
        }

        public static IServiceCollection WithValidators(this IServiceCollection builderServices)
        {
            builderServices.AddScoped<IValidator<LoginModel>, LoginModelValidator>();
            builderServices.AddScoped<IValidator<RegisterModel>, RegisterModelValidator>();
            builderServices.AddScoped<IValidator<ResendConfirmationModel>, ResendConfirmationModelValidator>();
            builderServices.AddScoped<IValidator<ResetPasswordModel>, ResetPasswordModelValidator>();

            return builderServices;
        }

        public static IServiceCollection WithCommon(this IServiceCollection builderServices)
        {
            builderServices.AddScoped<JsInterop>();
            builderServices.AddSyncfusionBlazor();
            builderServices.AddHttpClient();
            builderServices.AddHttpContextAccessor();

            return builderServices;
        }

        public static IServiceCollection WithIdentity(this IServiceCollection builderServices)
        {
            builderServices.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<CBTIdentityDataContext>().AddDefaultTokenProviders();
            
            builderServices.Configure<IdentityOptions>(options =>
            {
                // Default Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 0;

                options.SignIn.RequireConfirmedEmail = true;
                options.Lockout.MaxFailedAccessAttempts = 10;
            });

            builderServices.AddScoped<AuthenticationStateProvider, BaseAuthenticationStateProvider>();
            builderServices.AddScoped<BaseAuthenticationStateProvider>();

            return builderServices;
        }

        public static IServiceCollection WithWeb(this IServiceCollection builderServices)
        {
            builderServices.AddAuthentication();
            builderServices.AddAuthorization();

            builderServices.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.None;
                options.HttpOnly = HttpOnlyPolicy.None;
                options.Secure = CookieSecurePolicy.None;
                options.CheckConsentNeeded = _ => false;
            });

            builderServices.AddSignalR((options) =>
            {
                options.EnableDetailedErrors = true;
            });

            builderServices.AddControllers();

            builderServices
                .AddRazorComponents()
                .AddInteractiveServerComponents();

            return builderServices;
        }
    }
}

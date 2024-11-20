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
using CBT.SharedComponents.Blazor.Common;
using CBT.SharedComponents.Blazor.Model.Identity;
using CBT.SharedComponents.Blazor.Model.Validators;
using CBT.SharedComponents.Blazor.Services;
using CBT.SharedComponents.Blazor.Model;
using CBT.SharedComponents.Blazor.Model.Validators.Diaries;
using CBT.Logic.Services;

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

            builderServices.AddDbContextFactory<CBTIdentityDataContext>(options =>
            {
                options.UseSqlServer(databaseConfig?.SingleConnectionString);
            }, ServiceLifetime.Transient);
            builderServices.AddDbContextFactory<CBTDataContext>((options) =>
            {
                options.UseSqlServer(databaseConfig?.SingleConnectionString);
            }, ServiceLifetime.Transient);
            builderServices.AddDbContextFactory<CBTDataContextMARS>((options) =>
            {
                options.UseSqlServer(databaseConfig?.SingleConnectionStringMARS);
            }, ServiceLifetime.Transient);

            return builderServices;
        }

        public static IServiceCollection WithServices(this IServiceCollection builderServices)
        {
            builderServices.AddTransient<AutomaticThoughtsService>();
            builderServices.AddTransient<PeopleService>();
            builderServices.AddTransient<LinkingService>();
            builderServices.AddTransient<NotificationsService>();
            builderServices.AddTransient<SfDialogService>();
            builderServices.AddTransient<UserManager<User>>();

            builderServices.AddTransient<DiariesFacade>();
            builderServices.AddTransient<PsychologistReviewFacade>();
            builderServices.AddTransient<CognitiveErrorsFacade>();
            builderServices.AddTransient<EmotionsFacade>();
            builderServices.AddTransient<LinkingFacade>();

            builderServices.AddTransient<IEmailSender, EmailSender>();

            return builderServices;
        }

        public static IServiceCollection WithValidators(this IServiceCollection builderServices)
        {
            builderServices.AddScoped<IValidator<LoginModel>, LoginModelValidator>();
            builderServices.AddScoped<IValidator<RegisterModel>, RegisterModelValidator>();
            builderServices.AddScoped<IValidator<ResendConfirmationModel>, ResendConfirmationModelValidator>();
            builderServices.AddScoped<IValidator<ResetPasswordModel>, ResetPasswordModelValidator>();
            builderServices.AddScoped<IValidator<ProfileModel>, ProfileModelValidator>();

            builderServices.AddScoped<IValidator<ThreeColumnsTechniqueRecordModel>, ThreeColumnsTechniqueRecordModelValidator>();
            builderServices.AddScoped<IValidator<AutomaticThoughtDiaryRecordModel>, AutomaticThoughtDiaryRecordModelValidator>();

            return builderServices;
        }

        public static IServiceCollection WithCommon(this IServiceCollection builderServices)
        {
            builderServices.AddScoped<JsInterop>();
            builderServices.AddScoped<ClipboardService>();
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

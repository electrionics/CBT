﻿using CBT.Domain;
using CBT.Domain.Identity;
using CBT.Logic.Services;
using CBT.SharedComponents.Blazor.Model.Identity;
using CBT.SharedComponents.Blazor.Model.Validators;
using CBT.SharedComponents.Blazor.Services;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Popups;

namespace CBT.SharedComponents.Blazor
{
    public static class ServicesExtender
    {
        public static IServiceCollection Extend(this IServiceCollection builderServices, DatabaseConfig databaseConfig)
        {
            #region Database

            builderServices.AddSingleton(databaseConfig!);

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


            builderServices.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<CBTIdentityDataContext>().AddDefaultTokenProviders();

            #endregion

            #region Dependencies

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

            builderServices.AddScoped<IValidator<LoginModel>, LoginModelValidator>();
            builderServices.AddScoped<IValidator<RegisterModel>, RegisterModelValidator>();
            builderServices.AddScoped<IValidator<ResendConfirmationModel>, ResendConfirmationModelValidator>();
            builderServices.AddScoped<IValidator<ResetPasswordModel>, ResetPasswordModelValidator>();

            #endregion

            #region Web (possibly to remove from here)

            builderServices.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder => builder.WithOrigins("https://localhost:7040", "http://localhost:51979")
                                      .AllowAnyHeader()
                                      .AllowAnyMethod()
                                      .AllowCredentials());
            });

            builderServices.AddAuthentication();
            builderServices.AddAuthorization();

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

            builderServices.AddSignalR((options) =>
            {
                options.EnableDetailedErrors = true;
            });

            builderServices.AddControllers();
            builderServices.AddHttpClient();
            builderServices.AddHttpContextAccessor();

            // Add services to the container.
            builderServices.AddRazorPages();
            builderServices.AddServerSideBlazor();
            builderServices.AddSyncfusionBlazor();

            builderServices.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.None;
                options.HttpOnly = HttpOnlyPolicy.None;
                options.Secure = CookieSecurePolicy.None;
                options.CheckConsentNeeded = _ => false;
            });

            #endregion

            return builderServices;
        }
    }
}

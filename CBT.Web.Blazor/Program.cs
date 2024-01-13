using Syncfusion.Blazor;
using Syncfusion.Blazor.Popups;
using Microsoft.EntityFrameworkCore;
using CBT.Web.Blazor.Data.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Components.Authorization;
using CBT.Web.Blazor.Services;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("CBTIdentityDataContextConnection") ?? throw new InvalidOperationException("Connection string 'CBTIdentityDataContextConnection' not found.");

builder.Services.AddDbContext<CBTIdentityDataContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<CBTIdentityDataContext>(); //.AddDefaultTokenProviders()


builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSyncfusionBlazor();

builder.Services.AddSingleton<AutomaticThoughtsService>();
builder.Services.AddScoped<SfDialogService>();
builder.Services.AddScoped<UserManager<User>>();
//builder.Services.AddScoped<AuthenticationStateProvider, CBTAuthenticationStateProvider>();

var app = builder.Build();
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzAwNTIyM0AzMjM0MmUzMDJlMzBMWFFSeFJGTjRZRFJhL3JoYzNOdFRESjZ5Q05NcXJnckR1SFp0L0VwQUZFPQ==");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseAuthentication();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

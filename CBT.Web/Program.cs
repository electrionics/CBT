var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
// Add services to the container.

services.AddControllersWithViews();

services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        bld => bld
            .WithOrigins("https://localhost:7200")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

var app = builder.Build();
var env = app.Environment;

if (env.IsDevelopment())
{
    app.UseCors("CorsPolicy");
}

// Configure the HTTP request pipeline.
if (!env.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Blackberries.Models;
using Blackberries.Services;
using System.Globalization;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication()
    .AddCookie(options => {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;

        options.Cookie.Name = "Blackberries_auth";
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.SameSite = SameSiteMode.Lax;

        options.Events = new AuthenticationEvents();
    });

builder.Services.AddMvc();
builder.Services.AddOptions();
builder.Services.Configure<AdminUserConfig>(builder.Configuration.GetSection("AdminUserConfig"));
builder.Services.Configure<SmtpSettingsConfig>(builder.Configuration.GetSection("SmtpSettingsConfig"));

var app = builder.Build();

Blackberries.Services.ServiceProvider.Instance = app.Services;

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

//app.MapRazorPages();

//-- Default Route
app.MapControllerRoute(
    "default",
    "{controller}/{action}",
    new { controller = "Home", action = "Index"}
);

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("ru-RU"),
    SupportedCultures = new[] { new CultureInfo("ru-RU") },
    SupportedUICultures = new[] { new CultureInfo("ru-RU") }
});

app.Run();

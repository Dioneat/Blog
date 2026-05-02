using Blog10.Components;
using Blog10.Data;
using Blog10.Services;
using Blog10.Services.Admin;
using Blog10.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Radzen;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddRadzenComponents();
builder.Services.AddControllersWithViews();
builder.Services.AddLocalization();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBlogService, BlogService>();
builder.Services.AddScoped<IAboutService, AboutService>();
builder.Services.AddScoped<EncryptionService>();
builder.Services.AddScoped<SettingsService>();
builder.Services.AddMemoryCache();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/api/auth/logout";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
    });

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddHttpClient();

builder.Services.AddScoped<VkSyncService>();
builder.Services.AddScoped<TelegramSyncService>();
builder.Services.AddScoped<AppLogService>();
builder.Services.AddScoped<FileService>();
builder.Services.AddScoped<IFileService>(provider =>
    provider.GetRequiredService<FileService>());
builder.Services.AddScoped<IArticleAdminService, ArticleAdminService>();
builder.Services.AddScoped<IReviewAdminService, ReviewAdminService>();
builder.Services.AddScoped<IAboutAdminService, AboutAdminService>();

builder.Services.AddAntiforgery();
var supportedCultures = new[]
{
    new CultureInfo("ru-RU"),
};

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});
var localizationOptions = app.Services
    .GetRequiredService<IOptions<RequestLocalizationOptions>>()
    .Value;

app.UseRequestLocalization(localizationOptions);
if (!app.Environment.IsDevelopment())
{
    app.UseStatusCodePagesWithReExecute("/not-found");
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    context.Response.Headers["Permissions-Policy"] =
        "geolocation=(), microphone=(), camera=(), payment=(), usb=(), browsing-topics=()";

    await next();
});
app.UseHttpsRedirection();

app.UseStaticFiles();
app.MapStaticAssets();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapControllers();

app.Run();
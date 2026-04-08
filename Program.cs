using Blog10.Components;
using Blog10.Data;
using Blog10.Services; 
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Radzen;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddRadzenComponents();
builder.Services.AddControllers();
builder.Services.AddLocalization();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBlogService, BlogService>();
builder.Services.AddScoped<IAboutService, AboutService>();
builder.Services.AddScoped<EncryptionService>();
builder.Services.AddScoped<SettingsService>();



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


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.UseAuthentication(); 
app.UseAuthorization();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.MapControllers();
app.Run();

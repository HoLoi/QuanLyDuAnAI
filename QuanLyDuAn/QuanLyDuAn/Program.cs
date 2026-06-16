using System.Net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Data;
using QuanLyDuAn.Services;
using QuanLyDuAn.Services.Implementations;
using QuanLyDuAn.Services.Interfaces;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
builder.Services.Configure<AiApiOptions>(builder.Configuration.GetSection(AiApiOptions.SectionName));
builder.Services
    .AddOptions<AccountActivationOptions>()
    .Bind(builder.Configuration.GetSection(AccountActivationOptions.SectionName))
    .Validate(
        options => options.TokenLifetimeHours > 0,
        "AccountActivation:TokenLifetimeHours phải lớn hơn 0.")
    .Validate(
        options => options.ResendCooldownSeconds > 0,
        "AccountActivation:ResendCooldownSeconds phải lớn hơn 0.")
    .Validate(
        options =>
        {
            if (!Uri.TryCreate(options.AppBaseUrl?.Trim(), UriKind.Absolute, out var uri))
            {
                return false;
            }

            if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
            {
                return false;
            }

            if (uri.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (IPAddress.TryParse(uri.Host, out var ipAddress) && IPAddress.IsLoopback(ipAddress))
            {
                return false;
            }

            return true;
        },
        "AccountActivation:AppBaseUrl phải là URL HTTP/HTTPS tuyệt đối và không được dùng localhost/loopback.")
    .ValidateOnStart();

builder.Services.AddHttpClient<IAiApiService, AiApiService>((serviceProvider, client) =>
{
    var options = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<AiApiOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(Math.Max(1, options.TimeoutSeconds));
});

builder.Services.AddDbContext<QuanLyDuAnDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddTangDichVu();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<QuanLyDuAnDbContext>();
    await KhoiTaoTaiKhoanMacDinh.DamBaoDuLieuAsync(dbContext);
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.Run();

using System.Globalization;
using System.Threading.RateLimiting;
using ConectaTI;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("pt-BR");
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("pt-BR");
builder.Services.AddControllersWithViews(options => options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = "/participante";
    options.AccessDeniedPath = "/participante";
    options.Cookie.Name = "ConectaTI.Sessao";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.ExpireTimeSpan = TimeSpan.FromHours(2);
});
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = (context, token) => new ValueTask(context.HttpContext.Response
        .WriteAsync("Muitas tentativas. Aguarde um minuto e tente novamente.", token));
    options.AddPolicy("acesso", context => RateLimitPartition.GetFixedWindowLimiter(
        context.Connection.RemoteIpAddress?.ToString() ?? "local",
        _ => new FixedWindowRateLimiterOptions { PermitLimit = 15, Window = TimeSpan.FromMinutes(1) }));
});
builder.Services.AddSingleton<TimeProvider>(TimeProvider.System);
builder.Services.AddSingleton<IRepositorio>(sp =>
{
    var caminho = builder.Configuration["CONECTA_DADOS"] ?? Path.Combine(builder.Environment.ContentRootPath, "App_Data", "evento.json");
    return new RepositorioJson(Path.GetFullPath(caminho), sp.GetRequiredService<ILogger<RepositorioJson>>());
});
builder.Services.AddScoped<EventosService>();
var app = builder.Build();
app.UseExceptionHandler("/erro");
app.UseStatusCodePagesWithReExecute("/erro", "?status={0}");
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["Referrer-Policy"] = "no-referrer";
    context.Response.Headers["Content-Security-Policy"] = "default-src 'self'; frame-ancestors 'none'; base-uri 'self'; form-action 'self'";
    await next();
});
app.UseStaticFiles();
app.UseRouting();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

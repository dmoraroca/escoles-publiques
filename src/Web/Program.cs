using Application.Interfaces;
using Application.UseCases.Services;
using Domain.Interfaces;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Web.Hubs;
using Web.Validators;
using System.Globalization;
using Serilog;
using Web.Services.Api;
using Web.Services.Search;
using Application.Interfaces.Search;
using Application.UseCases.Queries.SearchResults;
using Web.Services.Search.Adapters;
using Microsoft.AspNetCore.Localization;

var builder = WebApplication.CreateBuilder(args);
// Configuració Serilog: logs per dia a /logs/logYYYYMMDD.log
var logPath = Path.Combine(AppContext.BaseDirectory, "logs", "log{Date}.log");
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
    .WriteTo.Console()
    .CreateLogger();
builder.Host.UseSerilog();

// Configurar cultura invariant per evitar problemes amb decimals
var cultureInfo = new CultureInfo("en-US");
cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
cultureInfo.NumberFormat.CurrencyDecimalSeparator = ".";
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

// Configurar PostgreSQL per timestamps
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

builder.Services.AddDbContext<SchoolDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<ISchoolRepository, SchoolRepository>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
builder.Services.AddScoped<IAnnualFeeRepository, AnnualFeeRepository>();
builder.Services.AddScoped<IScopeRepository, ScopeRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<ISchoolService, SchoolService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<IAnnualFeeService, AnnualFeeService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<SchoolViewModelValidator>();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
        ? CookieSecurePolicy.SameAsRequest
        : CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
});
builder.Services.AddHttpContextAccessor();

// HTTP client to call the API project
builder.Services.AddTransient<ApiAuthTokenHandler>();
var apiBaseUrl = builder.Configuration["Api:BaseUrl"] ?? "http://localhost:7000";
builder.Services.AddHttpClient<ISchoolsApiClient, SchoolsApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
}).AddHttpMessageHandler<ApiAuthTokenHandler>();

builder.Services.AddHttpClient<IAuthApiClient, AuthApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddHttpClient<IScopesApiClient, ScopesApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
}).AddHttpMessageHandler<ApiAuthTokenHandler>();

builder.Services.AddHttpClient<IStudentsApiClient, StudentsApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
}).AddHttpMessageHandler<ApiAuthTokenHandler>();

builder.Services.AddHttpClient<IEnrollmentsApiClient, EnrollmentsApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
}).AddHttpMessageHandler<ApiAuthTokenHandler>();

builder.Services.AddHttpClient<IAnnualFeesApiClient, AnnualFeesApiClient>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
}).AddHttpMessageHandler<ApiAuthTokenHandler>();

builder.Services.AddScoped<IScopeLookupSource, ScopeLookupSource>();
builder.Services.AddScoped<ISchoolSearchSource, SchoolSearchSource>();
builder.Services.AddScoped<IStudentSearchSource, StudentSearchSource>();
builder.Services.AddScoped<IEnrollmentSearchSource, EnrollmentSearchSource>();
builder.Services.AddScoped<IAnnualFeeSearchSource, AnnualFeeSearchSource>();
builder.Services.AddScoped<ISearchResultsQuery, SearchResultsQuery>();
builder.Services.AddScoped<ISearchResultsBuilder, SearchResultsBuilder>();

builder.Services.AddControllersWithViews(options =>
{
    // Requerir autenticació per defecte a tots els controladors
    var policy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter(policy));
})
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

builder.Services.AddSignalR();

// Configurar autenticació amb cookies
builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}
else
{
    app.Use(async (context, next) =>
    {
        context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
        context.Response.Headers["Pragma"] = "no-cache";
        context.Response.Headers["Expires"] = "0";
        await next();
    });
}
app.UseStaticFiles();

app.Use(async (context, next) =>
{
    var culture = context.Request.Query["culture"].ToString();
    var uiCulture = context.Request.Query["ui-culture"].ToString();
    if (!string.IsNullOrEmpty(culture))
    {
        var requestCulture = new RequestCulture(culture, string.IsNullOrEmpty(uiCulture) ? culture : uiCulture);
        context.Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(requestCulture));
    }
    await next();
});

// Forçar cultura invariant per a cada request
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("ca-ES"),
    SupportedCultures = new[]
    {
        new CultureInfo("ca-ES"),
        new CultureInfo("es-ES"),
        new CultureInfo("en-US"),
        new CultureInfo("de-DE"),
        new CultureInfo("fr-FR"),
        new CultureInfo("ru-RU"),
        new CultureInfo("zh-CN")
    },
    SupportedUICultures = new[]
    {
        new CultureInfo("ca-ES"),
        new CultureInfo("es-ES"),
        new CultureInfo("en-US"),
        new CultureInfo("de-DE"),
        new CultureInfo("fr-FR"),
        new CultureInfo("ru-RU"),
        new CultureInfo("zh-CN")
    }
});

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (UnauthorizedAccessException)
    {
        context.Session.Remove(Web.Services.Api.SessionKeys.ApiToken);
        var auth = context.RequestServices.GetRequiredService<IAuthenticationService>();
        await auth.SignOutAsync(context, "CookieAuth", null);
        context.Response.Redirect("/Auth/Login");
    }
});

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    endpoints.MapHub<SchoolHub>("/schoolHub");
});

app.Run();

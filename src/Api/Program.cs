using Application.Interfaces;
using Application.UseCases.Services;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Render provides PostgreSQL URLs like postgresql://user:pass@host/db which ADO.NET doesn't parse.
// Normalize to a standard Npgsql connection string.
static string NormalizePg(string? raw)
{
    if (string.IsNullOrWhiteSpace(raw)) return string.Empty;
    var s = raw.Trim();
    if (s.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase) ||
        s.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase))
    {
        var uri = new Uri(s);
        var userInfo = uri.UserInfo.Split(':', 2);
        var username = userInfo.Length > 0 ? Uri.UnescapeDataString(userInfo[0]) : "";
        var password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : "";
        var db = uri.AbsolutePath.TrimStart('/');

        var csb = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.IsDefaultPort ? 5432 : uri.Port,
            Database = db,
            Username = username,
            Password = password,
            // External hosts on Render require SSL; internal may not, but Prefer is safe.
            SslMode = SslMode.Prefer,
            TrustServerCertificate = true
        };

        // Respect sslmode=require if provided on the URL.
        var q = System.Web.HttpUtility.ParseQueryString(uri.Query);
        var sslmode = q.Get("sslmode");
        if (string.Equals(sslmode, "require", StringComparison.OrdinalIgnoreCase))
        {
            csb.SslMode = SslMode.Require;
        }

        return csb.ConnectionString;
    }

    return s;
}

var normalizedCs = NormalizePg(builder.Configuration.GetConnectionString("Default"));
if (!string.IsNullOrWhiteSpace(normalizedCs))
{
    builder.Configuration["ConnectionStrings:Default"] = normalizedCs;
}

// DbContext
builder.Services.AddDbContext<SchoolDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// Repositories & services (same as Web)
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

// Authentication - JWT
var jwtSection = builder.Configuration.GetSection("Jwt");
var key = jwtSection.GetValue<string>("Key") ?? "dev-secret-key-please-change";
var issuer = jwtSection.GetValue<string>("Issuer") ?? "EscolesApi";
var audience = jwtSection.GetValue<string>("Audience") ?? "EscolesClients";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = issuer,
        ValidateAudience = true,
        ValidAudience = audience,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
    };
});

builder.Services.AddAuthorization();

// CORS
var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
if (!builder.Environment.IsDevelopment() && (corsOrigins == null || corsOrigins.Length == 0))
{
    throw new InvalidOperationException(
        "CORS origins are not configured. Set Cors:AllowedOrigins (e.g. Cors__AllowedOrigins__0) for production.");
}
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCors", policy =>
    {
        if (corsOrigins is { Length: > 0 })
        {
            policy.WithOrigins(corsOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
        else
        {
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
    });
});

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Escoles API", Version = "v1" });
    options.TagActionsBy(api => new[] { api.ActionDescriptor.RouteValues["controller"] ?? "Default" });
    options.DocInclusionPredicate((name, api) => true);
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Introdueix: Bearer {token}"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Apply pending EF Core migrations on startup so a fresh database gets tables automatically.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SchoolDbContext>();
    db.Database.Migrate();
}

var swaggerEnabled = builder.Configuration.GetValue("Swagger:Enabled", app.Environment.IsDevelopment());
if (swaggerEnabled)
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.RoutePrefix = "api"; // serve Swagger UI at /api
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Escoles API V1");
        options.DocumentTitle = "Escoles API Docs";
    });
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseCors("DefaultCors");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Let the host URLs be controlled by environment (ASPNETCORE_URLS or docker ENV).
app.Run();

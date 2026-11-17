using Clinic.Application.Interfaces;
using Clinic.Application.Services.Authentication;
using Clinic.Domain.Identity;
using Clinic.Infrastructure.Authentication;
using Clinic.Infrastructure.Identity;
using Clinic.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;


var builder = WebApplication.CreateBuilder(args);

// Jwt  
builder.Services
    .AddOptions<JwtSettings>()
    .Bind(builder.Configuration.GetSection("JwtSettings"))
    .Validate(settings =>
        !string.IsNullOrWhiteSpace(settings.Issuer) &&
        !string.IsNullOrWhiteSpace(settings.Audience) &&
        !string.IsNullOrWhiteSpace(settings.SecretKey) &&
        settings.ExpirationInMinutes > 0,
        "JWT settings are missing or invalid.")
    .ValidateOnStart();


builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],

            ValidateAudience = true,
            ValidAudience = builder.Configuration["JwtSettings:Audience"],

            ValidateLifetime = true,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Convert.FromBase64String(
                    builder.Configuration["JwtSettings:SecretKey"]!)),

            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

builder.Services.AddScoped<IAuthService, AuthService>();

// Database
builder.Services.AddDbContext<ClinicDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ClinicDatabase")));

// Identity

builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
{
    // Password requirements
    options.Password.RequiredLength = 12;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;

    // Account lockouts
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.AllowedForNewUsers = true;

    // User account requirements
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ClinicDbContext>()
.AddDefaultTokenProviders();


// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Seeder Information
builder.Services.AddScoped<IdentitySeeder>();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<IdentitySeeder>();
    await seeder.SeedAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

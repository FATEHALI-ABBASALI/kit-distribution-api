using KitDistributionAPI.Data;
using KitDistributionAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database (MySQL)
var connectionString = builder.Configuration.GetConnectionString("MySql");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

// Services
builder.Services.AddSingleton<JwtService>();
builder.Services.AddSingleton<QrCodeService>();

// JWT Authentication
var jwtKey = "THIS_IS_SUPER_SECRET_KEY_FOR_KIT_DISTRIBUTION_2026";

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();


// 🔥🔥🔥 CORRECT CORS CONFIGURATION
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy
                .AllowAnyHeader()
                .AllowAnyMethod()
                .SetIsOriginAllowed(_ => true); // allow Railway + localhost + APK
        });
});

var app = builder.Build();


// Swagger
app.UseSwagger();
app.UseSwaggerUI();


// 🔥 IMPORTANT MIDDLEWARE ORDER
app.UseCors("AllowFrontend");   // must be BEFORE auth
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


// 🔥 Railway PORT binding (REQUIRED)
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Run($"http://0.0.0.0:{port}");

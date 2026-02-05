using KitDistributionAPI.Data;
using KitDistributionAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ===============================
// CONTROLLERS
// ===============================
builder.Services.AddControllers();


// ===============================
// DATABASE (MySQL)
// ===============================
var connectionString = builder.Configuration.GetConnectionString("MySql");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    )
);


// ===============================
// SERVICES
// ===============================
builder.Services.AddSingleton<JwtService>();
builder.Services.AddSingleton<QrCodeService>();


// ===============================
// JWT AUTHENTICATION
// ===============================
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


// ===============================
// AUTHORIZATION
// ===============================
builder.Services.AddAuthorization();


// ===============================
// CORS (ALLOW MOBILE / FRONTEND)
// ===============================
builder.Services.AddCors(o =>
{
    o.AddPolicy("AllowAll", p =>
        p.AllowAnyOrigin()
         .AllowAnyHeader()
         .AllowAnyMethod()
    );
});


// ===============================
// BUILD APP
// ===============================
var app = builder.Build();


// ===============================
// MIDDLEWARE ORDER
// ===============================
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


// ===============================
// 🚀 RAILWAY PORT BINDING (IMPORTANT)
// ===============================
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Run($"http://0.0.0.0:{port}");

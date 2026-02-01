using KitDistributionAPI.Data;
using KitDistributionAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ===============================
// LISTEN ON ALL IPs (🔥 REQUIRED FOR MOBILE)
// ===============================

// ===============================
// CONTROLLERS
// ===============================
builder.Services.AddControllers();

// ===============================
// DATABASE (MySQL)
// ===============================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("MySql"),
        ServerVersion.AutoDetect(
            builder.Configuration.GetConnectionString("MySql")
        )
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

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
// CORS (ALLOW MOBILE)
// ===============================
builder.Services.AddCors(o =>
{
    o.AddPolicy("AllowAll", p =>
        p.AllowAnyOrigin()
         .AllowAnyHeader()
         .AllowAnyMethod()
    );
});

var app = builder.Build();

// ===============================
// MIDDLEWARE ORDER (IMPORTANT)
// ===============================
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

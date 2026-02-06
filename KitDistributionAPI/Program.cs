using KitDistributionAPI.Data;
using KitDistributionAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();


// ================= SWAGGER =================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// ================= DATABASE =================
var connectionString = builder.Configuration.GetConnectionString("MySql");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);


// ================= SERVICES =================
builder.Services.AddSingleton<JwtService>();
builder.Services.AddSingleton<QrCodeService>();


// ================= JWT AUTH =================
var jwtKey = builder.Configuration["Jwt:Key"];

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();


// ================= CORS (IMPORTANT) =================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:3000",
                "https://localhost:3000"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


var app = builder.Build();


// ================= SWAGGER =================
app.UseSwagger();
app.UseSwaggerUI();


// ================= MIDDLEWARE ORDER (CRITICAL) =================
app.UseRouting();

app.UseCors("AllowFrontend");   // ⭐ MUST be before auth

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


// ================= RAILWAY PORT =================
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Run($"http://0.0.0.0:{port}");

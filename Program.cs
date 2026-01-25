using EShopAPI.DBContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// -------------------------------------------- DATABASE CONFIGURATION --------------------------------------------
// Registers AppDbContext using PostgreSQL.
// Connection string is loaded from appsettings.json -> "DefaultConnection".
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


// -------------------------------------------- CORS CONFIGURATION --------------------------------------------
// Allows requests from any origin, method, and header.
// Useful for MAUI, mobile apps, and local development.
// In production, you can restrict origins.
//
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});


// -------------------------------------------- CONTROLLERS --------------------------------------------
// Adds support for API controllers (AuthController, etc.)
builder.Services.AddControllers();

// -------------------------------------------- JWT AUTHENTICATION CONFIGURATION --------------------------------------------
// Reads JWT settings from appsettings.json -> "Jwt" section.
// Configures token validation for all protected endpoints.

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // How incoming JWT tokens should be validated
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,                     // Validate token issuer
            ValidateAudience = true,                   // Validate token audience
            ValidateIssuerSigningKey = true,           // Validate signing key
            ValidateLifetime = true,                   // Validate token expiration
            ValidIssuer = jwtSettings["Issuer"],       // Expected issuer
            ValidAudience = jwtSettings["Audience"],   // Expected audience
            IssuerSigningKey = new SymmetricSecurityKey(key) // Signing key
        };
    });



// -------------------------------------------- AUTHORIZATION --------------------------------------------
// Enables [Authorize] attribute support.

builder.Services.AddAuthorization();


// -------------------------------------------- BUILD APPLICATION --------------------------------------------
// Creates the WebApplication instance.
var app = builder.Build();


// -------------------------------------------- ROOT ENDPOINT (OPTIONAL) --------------------------------------------
// Simple health check endpoint to verify API is running.
app.MapGet("/", () => "API Online!");


// -------------------------------------------- MIDDLEWARE PIPELINE --------------------------------------------
// Order matters:
// 1. CORS
// 2. Authentication
// 3. Authorization
// 4. Controllers
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

// -------------------------------------------- MAP CONTROLLERS --------------------------------------------
// Enables routing for all controllers (e.g., /auth/login).
app.MapControllers();

// -------------------------------------------- RUN APPLICATION --------------------------------------------
// Starts the web server
app.Run();
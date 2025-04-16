using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PersonnelManagement.Data;
using PersonnelManagement.Repositories.Implementations;
using PersonnelManagement.Repositories.Interfaces;
using PersonnelManagement.Security;
using PersonnelManagement.Services.Implementations;
using PersonnelManagement.Services.Interfaces;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using System.Net.WebSockets;

var builder = WebApplication.CreateBuilder(args);

// Register logging
builder.Services.AddLogging();

// Connect DB
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()
    )
);

// Configure CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("https://localhost:7036",
                            "http://localhost:7036",
                            "http://localhost:3000",
                            "https://accounts.google.com",
                            "https://apilayer.khqi.site",
                            "https://hau.io.vn") // replace with your frontend port
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Configure authentication with JWT Bearer
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // For JWT
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;    // For JWT
    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme; // For JWT sign-in

})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
    };
});

builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ICccdRepository, CccdRepository>();
//builder.Services.AddScoped<IRoleAccountRepository, RoleAccountRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IFirebaseService, FirebaseService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        //options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        
    });
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseWebSockets();

app.UseHttpsRedirection();

// Place UseCors before UseAuthentication and UseAuthorization
app.UseCors("AllowAll");

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<RoleMiddleware>();

app.MapControllers();

app.Run();

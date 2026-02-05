using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using API.Data;
using API.Interface;
using API.Repository;
using API.Repository.Implementations;
using API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["SecretKey"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

// Configure Database Context
var connectionString = builder.Configuration.GetConnectionString("PostgreSQLDbConnStr")
    ?? builder.Configuration["ConnectionStrings:PostgreSQLDbConnStr"]
    ?? throw new InvalidOperationException("PostgreSQL connection string not configured.");

builder.Services.AddDbContext<MusicStreamingDbContext>(options =>
    options.UseNpgsql(connectionString));

// Register Repositories
// Register Repositories
builder.Services.AddScoped<IPlaylistRepository, PlaylistRepository>();
builder.Services.AddScoped<ITrackRepository, TrackRepository>();
builder.Services.AddScoped<IVenueRepository, VenueRepository>();
builder.Services.AddScoped<ITenantRepository, TenantRepository>();

// Register Services
builder.Services.AddScoped<IVenueService, VenueService>();
builder.Services.AddScoped<ITrackService, TrackService>();
builder.Services.AddScoped<IPlaylistService, PlaylistService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
// TODO: Implement StreamingService and CloudflareSignedUrlService
// builder.Services.AddScoped<IStreamingService, StreamingService>();
// builder.Services.AddScoped<ISignedUrlService, CloudflareSignedUrlService>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "B2B Music Streaming API v1");
        // Remove RoutePrefix override
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Interface;
using API.Repository;
using API.Repository.Implementations;
using API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Configure Database Context
// var connectionString = builder.Configuration["PostgreSQLDbConnStr"];
// builder.Services.AddDbContext<MusicStreamingDbContext>(options =>
//     options.UseNpgsql(connectionString));

// Register Repositories
// builder.Services.AddScoped<IPlaylistRepository, PlaylistRepository>();
// builder.Services.AddScoped<ITrackRepository, TrackRepository>();
// builder.Services.AddScoped<IVenueRepository, VenueRepository>();
// builder.Services.AddScoped<ITenantRepository, TenantRepository>();
// builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Register Services
// builder.Services.AddScoped<IVenueService, VenueService>();
// builder.Services.AddScoped<ITrackService, TrackService>();
// builder.Services.AddScoped<IPlaylistService, PlaylistService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
// TODO: Implement StreamingService and CloudflareSignedUrlService
// builder.Services.AddScoped<IStreamingService, StreamingService>();
// builder.Services.AddScoped<ISignedUrlService, CloudflareSignedUrlService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("AllowAllOrigins");

app.UseAuthorization();

app.MapControllers();

app.Run();

using System.Net;
using System.Text;
using System.Text.Json;
using BuildingBlocks.Cache;
using BuildingBlocks.Models;
using HotelService.Data;
using HotelService.Repositories;
using HotelService.Repositories.Interfaces;
using HotelService.Services;
using HotelService.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using UserService.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Swagger Config
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "HotelService", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] { }
        }
    });
});

#region Db Config
builder.Services.AddDbContext<HotelDbContext>(opts =>
{
    opts.UseSqlServer(builder.Configuration.GetConnectionString("HotelDB"));
});
#endregion

#region Redis Cache
builder.Services.AddStackExchangeRedisCache(opts =>
    opts.Configuration = builder.Configuration.GetConnectionString("Redis")
);
builder.Services.AddSingleton<ICacheService, RedisCacheService>();
#endregion

#region Register Services
builder.Services.AddScoped<IHotelService, HotelService.Services.HotelService>();
builder.Services.AddScoped<IHotelRepository, HotelRepository>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
#endregion

builder.Services.AddAuthorization();

#region JWT config
builder.Services.AddAuthentication(opts =>
{
    opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(opts =>
    {
        opts.SaveToken = true;
        opts.RequireHttpsMetadata = false;
        opts.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero, // Prevent extra valid time after expiry
            ValidAudience = builder.Configuration["JWT:ValidAudience"],
            ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
        };

        opts.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                context.HandleResponse(); // Skip default

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                var error = new Dictionary<string, string>
                {
                    { "Unauthorized", "Invalid or missing auth token." }
                };
                var result = JsonSerializer.Serialize(
                    ApiResponse<string>.Failed(error, "Unable to access", HttpStatusCode.Unauthorized)
                );

                return context.Response.WriteAsync(result);
            },
            OnForbidden = context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";
                var error = new Dictionary<string, string>
                {
                    { "Forbidden", "You don’t have permission." }
                };
                var result = JsonSerializer.Serialize(
                    ApiResponse<string>.Failed(error, "Unable to access", HttpStatusCode.Forbidden)
                );

                return context.Response.WriteAsync(result);
            }
        };
    });
#endregion

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<HotelDbContext>();
    try
    {
        int[] retryDelays = [1000, 2000, 5000];
        for (var i = 0; i <= retryDelays.Length; i++)
        {
            try { await db.Database.MigrateAsync(); break; }
            catch when (i < retryDelays.Length) { await Task.Delay(retryDelays[i]); }
        }
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Migration failed. Starting without applying migrations.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<GlobalExceptionHandler>(); // Custom Global Exception Handler

app.MapControllers();

app.Run();
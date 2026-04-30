using System.Net;
using System.Text;
using System.Text.Json;
using BookingService.Consumers;
using BuildingBlocks.Cache;
using BookingService.Data;
using BookingService.Repositories;
using BookingService.Repositories.Interfaces;
using BookingService.Services.Interfaces;
using BuildingBlocks.Models;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Polly;
using Polly.Extensions.Http;
using UserService.Helpers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BookingService", Version = "v1" });
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
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] { }
        }
    });
});

#region DB Config
builder.Services.AddDbContext<BookingDbContext>(opts =>
{
    opts.UseSqlServer(builder.Configuration.GetConnectionString("BookingDB"));
});
#endregion

#region Redis Cache
builder.Services.AddStackExchangeRedisCache(opts =>
    opts.Configuration = builder.Configuration.GetConnectionString("Redis"));
builder.Services.AddSingleton<ICacheService, RedisCacheService>();
#endregion

#region Register Services
builder.Services.AddScoped<IBookingService, BookingService.Services.BookingService>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
#endregion

#region JWT Config
builder.Services.AddAuthentication(opts =>
{
    opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(opts =>
{
    opts.SaveToken = true;
    opts.RequireHttpsMetadata = false;
    opts.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
    };

    opts.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            var error = new Dictionary<string, string> { { "Unauthorized", "Invalid or missing auth token." } };
            var result = JsonSerializer.Serialize(
                ApiResponse<string>.Failed(error, "Unable to access", HttpStatusCode.Unauthorized));
            return context.Response.WriteAsync(result);
        },
        OnForbidden = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";
            var error = new Dictionary<string, string> { { "Forbidden", "You don't have permission." } };
            var result = JsonSerializer.Serialize(
                ApiResponse<string>.Failed(error, "Unable to access", HttpStatusCode.Forbidden));
            return context.Response.WriteAsync(result);
        }
    };
});
#endregion

#region Polly Policies
static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}

static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(handledEventsAllowedBeforeBreaking: 3, durationOfBreak: TimeSpan.FromSeconds(10));
}

static IAsyncPolicy<HttpResponseMessage> GetFallbackPolicy()
{
    return Policy<HttpResponseMessage>
        .Handle<HttpRequestException>()
        .OrResult(r => !r.IsSuccessStatusCode)
        .FallbackAsync(async (ct) => new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
        {
            Content = new StringContent("Hotel service unavailable")
        });
}
#endregion

builder.Services.AddHttpClient("RoomService",
        client => { client.BaseAddress = new Uri("https://localhost:5000/gateway/room/"); })
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy())
    .AddPolicyHandler(GetFallbackPolicy());

builder.Services.AddHttpClient("UserService",
    client => { client.BaseAddress = new Uri("https://localhost:5000/gateway/user/"); });

builder.Services.AddHttpContextAccessor();

#region RabbitMQ Config
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<PaymentResultConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h => { });

        cfg.ReceiveEndpoint("booking-payment-success-queue", e =>
        {
            e.ConfigureConsumer<PaymentResultConsumer>(context);
        });

        cfg.ReceiveEndpoint("booking-payment-failed-queue", e =>
        {
            e.ConfigureConsumer<PaymentResultConsumer>(context);
        });
    });
});
#endregion

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<GlobalExceptionHandler>();

app.MapControllers();

app.Run();

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UserService.Data;
using UserService.Helpers;
using UserService.Models;
using UserService.Models.Configs;
using UserService.Repositories;
using UserService.Repositories.Interfaces;
using UserService.Services;
using UserService.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization();

#region MSSQL Config
builder.Services.AddDbContext<UserDbContext>(opts =>
{
    opts.UseSqlServer(builder.Configuration.GetConnectionString("UserDb"));
});
#endregion

#region Identity Config
builder.Services.AddIdentityCore<Users>(opts =>
{
    opts.SignIn.RequireConfirmedAccount = false;
    opts.Password.RequireDigit = false;
    opts.Password.RequiredLength = 6;
    opts.Password.RequireNonAlphanumeric = false;
    opts.Password.RequireUppercase = false;
    opts.Password.RequireLowercase = false;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<UserDbContext>()
    .AddDefaultTokenProviders();
#endregion

#region Register JWT Auth
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

        // need to keep this in apigateway instead of here
        // to make this logic centralized
        // opts.Events = new JwtBearerEvents
        // {
        //     OnChallenge = context =>
        //     {
        //         context.HandleResponse(); // Skip default
        //
        //         context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        //         context.Response.ContentType = "application/json";
        //         var error = new Dictionary<string, string>
        //         {
        //             { "Unauthorized", "Invalid or missing auth token." }
        //         };
        //         var result = JsonSerializer.Serialize(
        //             ApiResponse<string>.Failed(error, "Unable to access", HttpStatusCode.Unauthorized)
        //         );
        //
        //         return context.Response.WriteAsync(result);
        //     },
        //     OnForbidden = context =>
        //     {
        //         context.Response.StatusCode = StatusCodes.Status403Forbidden;
        //         context.Response.ContentType = "application/json";
        // var error = new Dictionary<string, string>
        // {
        // { "Forbidden", "You don’t have permission." }
        //         };
        //         var result = JsonSerializer.Serialize(
        //             ApiResponse<string>.Failed(error, "Unable to access", HttpStatusCode.Forbidden)
        //         );
        //
        //         return context.Response.WriteAsync(result);
        //     }
        // };
    });
#endregion

#region Register Repo and Services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService.Services.UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
#endregion

#region Hook JwtConfig values from appsettings
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JWT"));
#endregion


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<GlobalExceptionHandler>(); // Custom Global Exception Handler

app.Run();
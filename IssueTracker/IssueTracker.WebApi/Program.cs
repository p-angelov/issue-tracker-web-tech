using System.Text;
using IssueTracker.Core.Interfaces;
using IssueTracker.Core.Services;
using IssueTracker.Core.Settings;
using IssueTracker.Infrastructure.Data;
using IssueTracker.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace IssueTracker;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
        var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings!.Key)),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });
        
        builder.Services.AddDbContext<IssueTrackerDbContext>(options => 
            options.UseInMemoryDatabase("TicketsDb"));

        builder.Services.AddScoped<ITicketsRepository, TicketsRepository>();
        builder.Services.AddScoped<TicketsService>();
        builder.Services.AddScoped<IAuthService, AuthService>();

        builder.Services.AddControllers();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin",
                builder => builder.WithOrigins("http://localhost:3000")
                .AllowAnyMethod()
                .AllowAnyHeader());
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();
        
        app.UseCors("AllowSpecificOrigin");

        // Add authentication middleware
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
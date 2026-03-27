using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;
using System.Text;
using TaskPro.API.Filters;
using TaskPro.API.MiddleWares;
using TaskPro.Application;
using TaskPro.Application.Settings;
using TaskPro.Infraestructure;
using TaskPro.Infraestructure.Persistence.SQLDB;
using TaskPro.Infraestructure.Persistence.SQLDB.Seeder;

namespace TaskPro.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var logger = LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();
            try
            {
                var builder = WebApplication.CreateBuilder(args);
                builder.Logging.ClearProviders();
                builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                builder.Host.UseNLog();
                var jwtSettings = builder.Configuration
                    .GetSection("Jwt")
                    .Get<JwtSettings>()!;

                builder.Services
                    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = jwtSettings.Issuer,
                            ValidAudience = jwtSettings.Audience,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                            ClockSkew = TimeSpan.Zero,
                        };
                    });

                builder.Services.AddAuthorization();

                builder.Services.AddApplication(builder.Configuration);
                builder.Services.AddInfrastructure(builder.Configuration);

                builder.Services
                    .AddControllers(options =>
                    {
                        options.Filters.Add<ValidationFilter>();
                    })
                    .ConfigureApiBehaviorOptions(options =>
                    {
                        options.SuppressModelStateInvalidFilter = true;
                    });

                var allowedOrigins = builder.Configuration
                .GetSection("Cors:AllowedOrigins")
                .Get<string[]>() ?? [];

                builder.Services.AddCors(options =>
                    options.AddPolicy("Frontend", policy =>
                        policy
                            .WithOrigins(allowedOrigins)
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials()));

                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = "TaskPro API",
                        Version = "v1",
                    });

                    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header
                    });

                    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id   = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                    });
                });

                var app = builder.Build();

                app.UseMiddleware<ExceptionHandlingMiddleware>();

                app.UseSwagger();
                app.UseSwaggerUI();

                app.UseHttpsRedirection();
                app.UseCors("Frontend");
                app.UseAuthentication();
                app.UseAuthorization();
                app.MapControllers();

                using (var scope = app.Services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    await db.Database.MigrateAsync();
                    await DbSeeder.SeedAsync(db);
                }
                app.Run();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Application failed to start.");
                throw;
            }
            finally
            {
                LogManager.Shutdown();
            }
        }
    }
}

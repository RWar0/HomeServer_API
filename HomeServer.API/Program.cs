
using HomeServer.API.Commands;
using HomeServer.API.Configuration.RateLimiting;
using HomeServer.API.Middleware.ExceptionHandling;
using HomeServer.Application.Configuration.Jwt;
using HomeServer.Application.Interfaces.Aquariums;
using HomeServer.Application.Interfaces.Auth;
using HomeServer.Application.Interfaces.MyFiles;
using HomeServer.Application.Interfaces.ParameterChecks;
using HomeServer.Application.Interfaces.StoredFiles;
using HomeServer.Application.Interfaces.Users;
using HomeServer.Application.Interfaces.UserSession;
using HomeServer.Application.Interfaces.WaterChanges;
using HomeServer.Application.Services.Aquariums;
using HomeServer.Application.Services.Auth;
using HomeServer.Application.Services.MyFiles;
using HomeServer.Application.Services.ParameterChecks;
using HomeServer.Application.Services.StoredFiles;
using HomeServer.Application.Services.Users;
using HomeServer.Application.Services.UserSession;
using HomeServer.Application.Services.WaterChanges;
using HomeServer.Infrastructure.Data;
using HomeServer.Infrastructure.Security;
using HomeServer.Infrastructure.Storage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;

namespace HomeServer.API
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Authentication JWT implementation
            var jwtSettings = builder.Configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Access:Key"]!);

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
                            IssuerSigningKey = new SymmetricSecurityKey(key),

                            NameClaimType = JwtRegisteredClaimNames.Name,
                            RoleClaimType = ClaimTypes.Role
                        };
                        options.Events = new JwtBearerEvents
                        {
                            OnForbidden = async context =>
                            {
                                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                                context.Response.ContentType = "application/problem+json";

                                await context.Response.WriteAsJsonAsync(new ProblemDetails
                                {
                                    Status = StatusCodes.Status403Forbidden,
                                    Title = "Forbidden",
                                    Detail = "Nie masz wystarczaj¹cych uprawnieñ, ¿eby wykonaæ t¹ czynnoœæ!",
                                    Type = "https://tools.ietf.org/html/rfc9110#section-15.5.4",
                                });
                            }
                        };
                    });

            #endregion

            #region SQL Connection
            var dbConnectionString = builder.Configuration.GetConnectionString("DatabaseConnection");

            builder.Services.AddDbContext<MyDbContext>(options =>
                    options.UseNpgsql(
                        dbConnectionString,
                        b => b.MigrationsAssembly("HomeServer.Infrastructure")
                    ));
            #endregion

            builder.Services.AddApiRateLimiting(builder.Configuration);

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddMemoryCache();

            #region Services Initialization
            builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

            builder.Services.AddScoped<FileStorageService>();
            builder.Services.AddScoped<IStoredFilesService, StoredFilesService>();

            builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
            builder.Services.AddScoped<IJwtService, JwtService>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            builder.Services.AddScoped<IFileService, FileService>();

            builder.Services.AddScoped<IUsersService, UsersService>();
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

            builder.Services.AddScoped<IAquariumsService, AquariumsService>();
            builder.Services.AddScoped<IWaterChangesService, WaterChangesService>();
            builder.Services.AddScoped<IParameterChecksService, ParameterChecksService>();
            #endregion

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AngularPolicy", policy =>
                {
                    // Localhost policy
                    policy
                        .WithOrigins("http://192.168.0.111:4200", "http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                })
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var errors = context.ModelState
                            .Where(x => x.Value?.Errors.Count > 0)
                            .ToDictionary(
                                x => x.Key,
                                x => x.Value!.Errors.Select(error =>
                                {
                                    if (error.Exception is FormatException ||
                                        error.Exception?.InnerException is FormatException)
                                    {
                                        return x.Key switch
                                        {
                                            "Page" => "Strona musi byæ liczb¹ ca³kowit¹.",
                                            "PageSize" => "Rozmiar strony musi byæ liczb¹ ca³kowit¹.",
                                            "SortDirection" => "Nieprawid³owy kierunek sortowania.",
                                            _ => "Podano nieprawid³owy typ wartoœci."
                                        };
                                    }

                                    return string.IsNullOrEmpty(error.ErrorMessage)
                                        ? "Nieprawid³owa wartoœæ."
                                        : error.ErrorMessage;
                                }).ToArray()
                            );

                        return new BadRequestObjectResult(new { errors });
                    };
                });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();

            var app = builder.Build();
            if (args.Length > 0)
            {
                return await CommandRunner.RunAsync(args, app.Services);
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("AngularPolicy");

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseRateLimiter();
            app.UseAuthorization();

            app.UseExceptionHandler();

            app.MapControllers();

            await app.RunAsync();
            return 0;
        }
    }
}

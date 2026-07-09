
using HomeServer.API.Configuration.RateLimiting;
using HomeServer.API.Middleware.ExceptionHandling;
using HomeServer.Application.Interfaces.Aquariums;
using HomeServer.Application.Interfaces.Auth;
using HomeServer.Application.Interfaces.StoredFiles;
using HomeServer.Application.Interfaces.Users;
using HomeServer.Application.Interfaces.UserSession;
using HomeServer.Application.Services.Aquariums;
using HomeServer.Application.Services.Auth;
using HomeServer.Application.Services.StoredFiles;
using HomeServer.Application.Services.Users;
using HomeServer.Application.Services.UserSession;
using HomeServer.Infrastructure.Data;
using HomeServer.Infrastructure.Security;
using HomeServer.Infrastructure.Storage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace HomeServer.API
{
    public class Program
    {
        public static void Main(string[] args)
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
                            IssuerSigningKey = new SymmetricSecurityKey(key)
                        };
                    });

            #endregion

            #region SQL Connection
            var dbConnectionString = builder.Configuration.GetConnectionString("DatabaseConnection");

            builder.Services.AddDbContext<MyDbContext>(options =>
                    options.UseSqlServer(
                        dbConnectionString,
                        b => b.MigrationsAssembly("HomeServer.Infrastructure")
                    ));
            #endregion

            builder.Services.AddApiRateLimiting(builder.Configuration);

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddMemoryCache();

            #region Services Initialization

            builder.Services.AddScoped<FileStorageService>();
            builder.Services.AddScoped<IStoredFilesService, StoredFilesService>();

            builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
            builder.Services.AddScoped<IJwtService, JwtService>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            builder.Services.AddScoped<IUsersService, UsersService>();
            builder.Services.AddScoped<IAquariumsService, AquariumsService>();


            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

            #endregion

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseRateLimiter();
            app.UseAuthorization();

            app.UseExceptionHandler();

            app.MapControllers();

            app.Run();
        }
    }
}

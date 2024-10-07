using Identity.Core.Repositories;
using Identity.Persistence;
using Identity.Security.Hashing;
using Identity.Core.Security.Tokens;
using Identity.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Identity.Core.Services;
using Identity.Core.Security.Hashing;
using Microsoft.Extensions.Hosting;

namespace Identity.Extensions
{
    public static class IdentityExtension
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration, bool isInMemoryMode = false)
        {
            if (isInMemoryMode)
            {
                services.AddDbContext<AuthDbContext>(options =>
                {
                    options.UseInMemoryDatabase("jwtapi");
                });
            }
            else
            {
                services.AddDbContext<AuthDbContext>(options =>
                {
                    options.UseSqlServer(configuration.GetConnectionString("AuthDbConnection"),x=>x.MigrationsAssembly(typeof(IdentityExtension).Assembly.FullName));
                });
            }

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddSingleton<IPasswordHasher, PasswordHasher>();
            services.AddSingleton<ITokenHandler, Core.Security.Tokens.TokenHandler>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();

            services.Configure<TokenOptions>(configuration.GetSection("TokenOptions"));
            var tokenOptions = configuration.GetSection("TokenOptions").Get<TokenOptions>();

            var signingConfigurations = new SigningConfigurations(tokenOptions.Secret);
            services.AddSingleton(signingConfigurations);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(jwtBearerOptions =>
                {
                    jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = tokenOptions.Issuer,
                        ValidAudience = tokenOptions.Audience,
                        IssuerSigningKey = signingConfigurations.SecurityKey,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            services.AddAutoMapper(typeof(IdentityExtension).Assembly);

            return services;
        }

        public static void SeedDefaultDbData(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetService<AuthDbContext>();
                var passwordHasher = services.GetService<IPasswordHasher>();
                DatabaseSeed.Seed(context, passwordHasher);
            }
        }
    }
}

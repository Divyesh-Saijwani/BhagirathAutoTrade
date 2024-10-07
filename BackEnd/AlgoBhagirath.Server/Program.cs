using Domain.Repositories;
using Microsoft.OpenApi.Models;
using Persistance.Repositories;
using Persistance;
using Services;
using Services.Abstractions;
using IciciDirectBreeze.Extension;
using Identity.Extensions;

namespace AlgoBhagirath.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

            builder.Services.AddIciciDirectBreezeMarketData(builder.Configuration);

            builder.Services.AddJwtAuthentication(builder.Configuration, true);

            builder.Services.AddScoped<IServiceManager, ServiceManager>();

            builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();

            builder.Services.AddSingleton<RepositoryDbContext>();

            var CORSOpenPolicy = "AlgoBhagirathCORSPolicy";

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(
                  name: CORSOpenPolicy,
                  policy => {
                      policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                  });
            });

            builder.Services.AddControllers();

            builder.Services.AddControllers()
                .AddApplicationPart(typeof(Presentation.AssemblyReference).Assembly);

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            //builder.Services.AddSwaggerGen();

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ALgoBhagirath API", Version = "v1" });
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "JSON Web Token to access resources. Example: Bearer {token}",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        new [] { string.Empty }
                    }
                });
            });

            var app = builder.Build();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AlgoBhagirath API v1"));
            //}

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();
            app.UseCors(CORSOpenPolicy);

            app.MapControllers();

            //app.MapFallbackToFile("/index.html");

            app.Run();
        }
    }
}

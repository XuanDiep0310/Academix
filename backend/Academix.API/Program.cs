
using Academix.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

namespace Academix.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            #region Configurating  Services - Start
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<AcademixDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DbContext"),
                    providerOptions => providerOptions.EnableRetryOnFailure());
            });

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            #endregion Configurating  Services - End

            #region Configurating  Middleware - Start
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference(options =>
                {
                    options.WithTitle("Academix API");
                    options.WithTheme(ScalarTheme.BluePlanet);
                    options.WithSidebar(true);
                });

                app.UseSwaggerUi(options =>
                {
                    options.DocumentPath = "openapi/v1.json";
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
            #endregion Configurating  Middleware - Start
        }
    }
}

using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;
using TaskList.Logic.Helpers;
using TaskList.Logic.Helpers.Interfaces;
using TaskList.WebApi.Managers;
using TaskList.WebApi.Managers.Interfaces;

namespace TaskList.WebApi
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            WebApplication.CreateBuilder(args)
                // Dependency Injection
                .RegisterApiServices()
                .RegisterNetServices()
                // Configuration
                .Configure()
                // Start the application
                .Run();
        }

        #region Dependency Injection
        private static WebApplicationBuilder RegisterApiServices(this WebApplicationBuilder builder)
        {
            // Singletons
            builder.Services.TryAddSingleton<ICounterRegister, CounterRegister>();
            builder.Services.TryAddSingleton<IWebApiTaskManager, WebApiTaskManager>();

            return builder;
        }

        private static WebApplicationBuilder RegisterNetServices(this WebApplicationBuilder builder)
        {
            _ = builder.Services.AddControllers();
            _ = builder.Services.AddEndpointsApiExplorer();
            _ = builder.Services.AddSwaggerGen();

            return builder;
        }
        #endregion

        #region Configuration
        private static WebApplication Configure(this WebApplicationBuilder builder)
        {
            // Add the XML documentation to the API Endpoints in Swagger UI
            _ = builder.Services.AddSwaggerGen(options =>
            {
                string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            });

            WebApplication webApplication = builder.Build();

            // Configure the HTTP request pipeline
            if (webApplication.Environment.IsDevelopment())
            {
                _ = webApplication.UseSwagger();
                _ = webApplication.UseSwaggerUI();
            }

            _ = webApplication.UseHttpsRedirection();

            _ = webApplication.UseAuthorization();

            _ = webApplication.MapControllers();

            return webApplication;
        }
        #endregion
    }
}

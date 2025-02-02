using System.Reflection;

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
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            return builder;
        }
        #endregion

        #region Configuration
        private static WebApplication Configure(this WebApplicationBuilder builder)
        {
            // Add the XML documentation to the API Endpoints in Swagger UI
            builder.Services.AddSwaggerGen(options =>
            {
                string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            });

            WebApplication webApplication = builder.Build();

            // Configure the HTTP request pipeline
            if (webApplication.Environment.IsDevelopment())
            {
                webApplication.UseSwagger();
                webApplication.UseSwaggerUI();
            }

            webApplication.UseHttpsRedirection();

            webApplication.UseAuthorization();

            webApplication.MapControllers();

            return webApplication;
        }
        #endregion
    }
}

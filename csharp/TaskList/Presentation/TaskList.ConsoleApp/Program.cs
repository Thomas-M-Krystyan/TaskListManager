using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TaskList.ConsoleApp.Controllers;
using TaskList.ConsoleApp.Controllers.Interfaces;
using TaskList.ConsoleApp.IO;
using TaskList.ConsoleApp.IO.Interfaces;
using TaskList.ConsoleApp.Managers.Interfaces;
using TaskList.ConsoleApp.Managers;

namespace TaskList.ConsoleApp
{
    internal static class Program
    {
        /// <summary>
        /// The entry point of the application.
        /// </summary>
        public static void Main(string[] _)
        {
            ServiceProvider serviceProvider = new ServiceCollection()
                // Dependency Injection
                .RegisterConsoleServices()
                .BuildServiceProvider();

            // Start the application
            serviceProvider.GetRequiredService<IWorkflowController>().Run();
        }

        #region Dependency Injection
        private static IServiceCollection RegisterConsoleServices(this ServiceCollection services)
        {
            // Singletons
            services.TryAddSingleton<IConsoleTaskManager, ConsoleTaskManager>();

            // Scoped
            return services
                .AddScoped<IConsole, RealConsole>()
                .AddScoped<IWorkflowController, TaskListController>();
        }
        #endregion
    }
}

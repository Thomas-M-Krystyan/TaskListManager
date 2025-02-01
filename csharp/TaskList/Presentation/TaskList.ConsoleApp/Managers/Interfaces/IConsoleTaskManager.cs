using TaskList.Logic.Managers.Interfaces;

namespace TaskList.ConsoleApp.Managers.Interfaces
{
    /// <summary>
    ///   <inheritdoc cref="ITaskManager"/>
    /// </summary>
    /// <remarks>
    ///   The console-specific implementation.
    /// </remarks>
    internal interface IConsoleTaskManager : ITaskManager
    {
        /// <summary>
        /// Displays the available commands.
        /// </summary>
        public string Help();

        /// <summary>
        /// Displays error about invalid command.
        /// </summary>
        /// <param name="command">The command.</param>
        public string Error(string command);
    }
}

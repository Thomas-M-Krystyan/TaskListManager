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
    }
}

using System.Text;
using TaskList.ConsoleApp.Managers.Interfaces;
using TaskList.Domain.Models;
using TaskList.Logic.Helpers.Interfaces;
using TaskList.Logic.Managers;
using TaskList.Logic.Managers.Interfaces;
using TaskList.Logic.Responses;

namespace TaskList.ConsoleApp.Managers
{
    /// <inheritdoc cref="IConsoleTaskManager"/>
    /// <seealso cref="ITaskManager"/>
    internal sealed class ConsoleTaskManager : ConcurrentTaskManager, IConsoleTaskManager
    {
        private readonly StringBuilder _stringBuilder = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleTaskManager"/> class.
        /// </summary>
        public ConsoleTaskManager(ICounterRegister counter) : base(counter)
        {
        }

        /// <inheritdoc cref="ITaskManager.DisplayTaskList()"/>
        public override CommandResponse DisplayTaskList()
        {
            try
            {
                _stringBuilder.Clear();

                // NOTE: Order of elements is not guaranteed in a ConcurrentDictionary. Even if the order of elements doesn't
                //       matter at all from the perspective of business logic, it's important condition for the functional test
                foreach (KeyValuePair<string, ProjectItem> project in GetTaskList())
                {
                    // Project name
                    _stringBuilder.AppendLine(project.Value.Name);

                    foreach (TaskItem task in project.Value.Tasks)
                    {
                        // Task description
                        _stringBuilder.AppendLine(string.Format("    [{0}] {1}: {2}", task.IsDone ? 'x' : ' ', task.Id, task.Description));
                    }

                    _stringBuilder.AppendLine();
                }

                return CommandResponse.Success(_stringBuilder.ToString(), true);
            }
            catch (Exception exception)
            {
                return CommandResponse.Failure(exception);
            }
        }
    }
}

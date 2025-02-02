using TaskList.ConsoleApp.IO.Interfaces;
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
        private readonly IStringBuilder _stringBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleTaskManager"/> class.
        /// </summary>
        public ConsoleTaskManager(IStringBuilder stringBuilder, ICounterRegister counter) : base(counter)
        {
            _stringBuilder = stringBuilder;
        }

        /// <inheritdoc cref="ITaskManager.DisplayAllTasks()"/>
        public override CommandResponse DisplayAllTasks()
        {
            try
            {
                _stringBuilder.Clear();

                foreach (KeyValuePair<string, ProjectItem> project in GetProjectsSortedById())
                {
                    // Project name
                    _stringBuilder.AppendLine(project.Value.Name);

                    foreach (KeyValuePair<long, TaskItem> task in project.Value.Tasks.OrderBy(task => task.Value.Id))
                    {
                        // Task description
                        _stringBuilder.AppendLine(GetTaskDetails(task.Value));
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

        /// <inheritdoc cref="ITaskManager.DisplayTodayTasks()"/>
        public override CommandResponse DisplayTodayTasks()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc cref="IConsoleTaskManager.Help()"/>
        public string Help()
        {
            const string helpMessage =
                $"Commands:\r\n" +
                "  show\r\n" +
                "  add project <project name>\r\n" +
                "  add task <project name> <task description>\r\n" +
                "  check <task ID>\r\n" +
                "  uncheck <task ID>\r\n" +
                "  deadline <task ID> <deadline>\r\n" +
                "  quit\r\n";

            return helpMessage;
        }

        /// <inheritdoc cref="IConsoleTaskManager.Error(string)"/>
        public string Error(string command)
        {
            return string.Format("I don't know what the command \"{0}\" is.", command);
        }

        #region Helper methods
        // NOTE: Order of elements is not guaranteed in a ConcurrentDictionary. Even if the order of elements doesn't
        //       matter at all from the perspective of business logic, it might be important from the user perspective
        private IOrderedEnumerable<KeyValuePair<string, ProjectItem>> GetProjectsSortedById()
            => GetTaskList().OrderBy(project => project.Value.Id);

        private static string GetTaskDetails(TaskItem task)
            => string.Format("    [{0}] {1}: {2}", task.IsDone ? 'x' : ' ', task.Id, task.Name);
        #endregion
    }
}

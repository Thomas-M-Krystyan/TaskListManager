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
        private readonly HashSet<string> _todayProjects = [];

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

                Dictionary<long, TaskItem> tasks = GetTasksSortedById(GetAllTasks());

                foreach (KeyValuePair<string, ProjectItem> project in GetProjectsSortedById(GetAllProjects()))
                {
                    // Project name
                    _stringBuilder.AppendLine(project.Value.Name);

                    foreach (long taskId in project.Value.TaskIds)
                    {
                        // Task description
                        _stringBuilder.AppendLine(GetTaskDetails(tasks[taskId]));
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
            try
            {
                _stringBuilder.Clear();
                _todayProjects.Clear();

                DateOnly today = DateOnly.FromDateTime(DateTime.Today);

                Dictionary<long, TaskItem> todayTasks = GetTasksSortedById(GetAllTasks()
                    .Where(task => task.Value.Deadline.Equals(today)));

                foreach (KeyValuePair<long, TaskItem> task in todayTasks)
                {
                    if (_todayProjects.Add(task.Value.ProjectName))
                    {
                        // Project name
                        _stringBuilder.AppendLine();
                        _stringBuilder.AppendLine(task.Value.ProjectName);
                    }

                    // Task description
                    _stringBuilder.AppendLine(GetTaskDetails(task.Value));
                }

                _stringBuilder.AppendLine();

                return CommandResponse.Success(_stringBuilder.ToString().TrimStart(), true);
            }
            catch (Exception exception)
            {
                return CommandResponse.Failure(exception);
            }
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
                "  today\r\n" +
                "  help\r\n" +
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
        private static KeyValuePair<string, ProjectItem>[] GetProjectsSortedById(IReadOnlyDictionary<string, ProjectItem> taskList)
            => [.. taskList.OrderBy(project => project.Value.OrderNumber)];

        private static Dictionary<long, TaskItem> GetTasksSortedById(IEnumerable<KeyValuePair<long, TaskItem>> tasks)
            => tasks.OrderBy(task => task.Value.Id).ToDictionary();

        private static string GetTaskDetails(TaskItem task)
            => string.Format("    [{0}] {1}: {2}", task.IsDone ? 'x' : ' ', task.Id, task.Name);
        #endregion
    }
}

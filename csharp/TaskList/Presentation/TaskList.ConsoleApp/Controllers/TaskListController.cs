using TaskList.ConsoleApp.Controllers.Interfaces;
using TaskList.ConsoleApp.IO.Interfaces;
using TaskList.Domain.Models;

namespace TaskList.ConsoleApp.Controllers
{
    /// <inheritdoc cref="IWorkflowController"/>
    internal sealed class TaskListController : IWorkflowController
    {
        private const string QUIT = "quit";
        public const string StartupText = "Welcome to TaskList! Type 'help' for available commands.";

        private readonly IConsole _console;
        private readonly Dictionary<string, ProjectItem> _tasks = [];

        private long _lastId = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskListController"/> class.
        /// </summary>
        public TaskListController(IConsole console)
        {
            _console = console;
        }

        /// <inheritdoc cref="IWorkflowController.Run()"/>
        public void Run()
        {
            _console.WriteLine(StartupText);

            while (true)
            {
                try
                {
                    _console.Write("> ");

                    string command = _console.ReadLine();

                    if (command.Equals(QUIT, StringComparison.InvariantCultureIgnoreCase))
                    {
                        break;
                    }

                    Execute(command);
                }
                catch (Exception exception)
                {
                    _console.WriteLine(exception.Message);
                }
            }
        }

        #region Controller
        private void Execute(string commandLine)
        {
            string[] commandRest = commandLine.Split(' ', 2);
            string command = commandRest[0];

            switch (command.ToLower())
            {
                case "show":
                    ShowCommand();
                    break;

                case "add":
                    AddCommand(commandRest[1]);
                    break;

                case "check":
                    SetDoneCommand(commandRest[1], true);
                    break;

                case "uncheck":
                    SetDoneCommand(commandRest[1], false);
                    break;

                case "help":
                    HelpCommand();
                    break;

                default:
                    ErrorCommand(command);
                    break;
            }
        }
        #endregion

        #region Commands
        private void ShowCommand()
        {
            foreach (KeyValuePair<string, ProjectItem> project in _tasks)
            {
                _console.WriteLine(project.Key);

                foreach (TaskItem task in project.Value.Tasks)
                {
                    _console.WriteLine("    [{0}] {1}: {2}", task.IsDone ? 'x' : ' ', task.Id, task.Description);
                }

                _console.WriteLine();
            }

            // TODO: The status of the operation could be used for UI/UX purposes
        }

        private void AddCommand(string commandLine)
        {
            string[] subcommandRest = commandLine.Split(' ', 2);
            string subcommand = subcommandRest[0];

            if (subcommand == "project")
            {
                AddProject(subcommandRest[1]);

                // TODO: The status of the operation could be used for UI/UX purposes
            }
            else if (subcommand == "task")
            {
                string[] projectTask = subcommandRest[1].Split(' ', 2);

                AddTask(projectTask[0], projectTask[1]);

                // TODO: More statuses of the operations could be used for UI/UX purposes
            }

            // TODO: Invalid subcommand (for other cases than "project" and "task") should be reported to the used
        }

        private void AddProject(string name)
        {
            _tasks[name] = new ProjectItem { Name = name, Tasks = [] };

            // TODO: The status of the operation could be used for UI/UX purposes
        }

        private void AddTask(string project, string description)
        {
            if (_tasks.TryGetValue(project, out ProjectItem projectTasks))
            {
                projectTasks.Tasks.Add(new TaskItem { Id = NextId(), Description = description, IsDone = false });
            }
            else
            {
                Console.WriteLine("Could not find a project with the name \"{0}\".", project);
            }

            // TODO: The status of the operation could be used for UI/UX purposes
        }

        private void SetDoneCommand(string idString, bool isDone)
        {
            if (long.TryParse(idString, out long taskId))
            {
                TaskItem? identifiedTask = _tasks
                    .Select(project => project.Value.Tasks.FirstOrDefault(task => task.Id == taskId))
                    .Where(task => task != null)
                    .FirstOrDefault();

                if (identifiedTask == null)
                {
                    _console.WriteLine("Could not find a task with an ID of {0}.", taskId);

                    return;
                }

                identifiedTask.IsDone = isDone;
            }

            // TODO: The status of the operation could be used for UI/UX purposes
        }

        private void HelpCommand()
        {
            _console.WriteLine("Commands:");
            _console.WriteLine("  show");
            _console.WriteLine("  add project <project name>");
            _console.WriteLine("  add task <project name> <task description>");
            _console.WriteLine("  check <task ID>");
            _console.WriteLine("  uncheck <task ID>");
            _console.WriteLine("  quit");
            _console.WriteLine();
        }

        private void ErrorCommand(string command)
        {
            _console.WriteLine("I don't know what the command \"{0}\" is.", command);
        }

        private long NextId()
        {
            return ++_lastId;
        }
        #endregion
    }
}

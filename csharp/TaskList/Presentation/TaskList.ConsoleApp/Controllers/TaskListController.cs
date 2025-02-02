using TaskList.ConsoleApp.Controllers.Interfaces;
using TaskList.ConsoleApp.IO.Interfaces;
using TaskList.ConsoleApp.Managers.Interfaces;
using TaskList.Logic.Extensions;
using TaskList.Logic.Responses;

namespace TaskList.ConsoleApp.Controllers
{
    /// <inheritdoc cref="IWorkflowController"/>
    internal sealed class TaskListController : IWorkflowController
    {
        private const string QUIT = "quit";
        public const string StartupText = "Welcome to TaskList! Type 'help' for available commands.";

        private readonly IConsole _console;
        private readonly IConsoleTaskManager _taskManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskListController"/> class.
        /// </summary>
        public TaskListController(IConsole console, IConsoleTaskManager taskManager)
        {
            _console = console;
            _taskManager = taskManager;
        }

        /// <inheritdoc cref="IWorkflowController.Run()"/>
        public void Run()
        {
            _console.WriteLine(StartupText);

            string command = string.Empty;

            while (true)
            {
                try
                {
                    _console.Write("> ");

                    command = _console.ReadLine();

                    if (command.Equals(QUIT, StringComparison.InvariantCultureIgnoreCase))
                    {
                        break;
                    }

                    Execute(command);
                }
                catch (Exception exception)
                {
                    _console.WriteLine(_taskManager.Error(command));
                    _console.WriteLine(exception.GetMessage());
                }
            }
        }

        #region Controller
        private void Execute(string commandLine)
        {
            string[] commandRest = commandLine.Split(' ', 2);
            string command = commandRest[0];

            switch (command.ToLower())  // TODO: This can be improved by using Spans or if-else statements. ToLower() is working but introducing overhead
            {
                case "show":
                    ShowAllCommand();
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

                case "deadline":
                    SetDeadlineCommand(commandRest[1]);
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
        private void ShowAllCommand()
        {
            _console.Write(_taskManager.DisplayAllTasks().Content);

            // TODO: The status of the operation could be used for UI/UX purposes
        }

        private void AddCommand(string commandLine)
        {
            string[] subcommandRest = commandLine.Split(' ', 2);
            string subcommand = subcommandRest[0];

            if (subcommand.Equals("project", StringComparison.InvariantCultureIgnoreCase))
            {
                _ = _taskManager.AddProject(subcommandRest[1]);

                // TODO: The status of the operation could be used for UI/UX purposes
            }
            else if (subcommand.Equals("task", StringComparison.InvariantCultureIgnoreCase))
            {
                string[] subcommands = subcommandRest[1].Split(' ', 2);
                string projectName = subcommands[0];
                string taskName = subcommands[1];

                CommandResponse result = _taskManager.AddTask(projectName, taskName);

                if (result.IsFailure)
                {
                    _console.WriteLine(result.Content);
                }

                // TODO: More statuses of the operations could be used for UI/UX purposes
            }

            // TODO: Invalid subcommand (for other cases than "project" and "task") should be reported to the used
        }

        private void SetDoneCommand(string idString, bool isDone)
        {
            if (long.TryParse(idString, out long taskId))
            {
                CommandResponse result = _taskManager.CheckTask(taskId, isDone);

                if (result.IsFailure)
                {
                    _console.WriteLine(result.Content);
                }
            }

            // TODO: The status of the operation could be used for UI/UX purposes
        }

        private void SetDeadlineCommand(string commandLine)
        {
            string[] subcommands = commandLine.Split(' ', 2);
            string taskStringId = subcommands[0];
            string taskStringDeadline = subcommands[1];

            if (long.TryParse(taskStringId, out long taskId) &&
                DateOnly.TryParse(taskStringDeadline, out DateOnly deadline))
            {
                CommandResponse result = _taskManager.SetDeadline(taskId, deadline);

                if (result.IsFailure)
                {
                    _console.WriteLine(result.Content);
                }
            }

            // TODO: The status of the operation could be used for UI/UX purposes
        }

        private void HelpCommand()
        {
            _console.WriteLine(_taskManager.Help());
        }

        private void ErrorCommand(string command)
        {
            _console.WriteLine(_taskManager.Error(command));
        }
        #endregion
    }
}

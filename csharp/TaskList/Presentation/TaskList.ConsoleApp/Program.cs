using TaskList.ConsoleApp.IO;
using TaskList.ConsoleApp.IO.Interfaces;
using TaskList.Domain.Models;

namespace TaskList.ConsoleApp
{
    internal class Program
    {
        private const string QUIT = "quit";
        public static readonly string startupText = "Welcome to TaskList! Type 'help' for available commands.";

        private readonly Dictionary<string, ProjectItem> _tasks = [];
        private readonly IConsole _console;

        private long _lastId = 0;

        public static void Main(string[] args)
        {
            new Program(new RealConsole()).Run();
        }

        public Program(IConsole console)
        {
            _console = console;
        }

        public void Run()
        {
            _console.WriteLine(startupText);

            while (true)
            {
                _console.Write("> ");

                string command = _console.ReadLine();

                if (command == QUIT)
                {
                    break;
                }

                Execute(command);
            }
        }

        private void Execute(string commandLine)
        {
            string[] commandRest = commandLine.Split(" ".ToCharArray(), 2);
            string command = commandRest[0];

            switch (command)
            {
                case "show":
                    Show();
                    break;

                case "add":
                    Add(commandRest[1]);
                    break;

                case "check":
                    Check(commandRest[1]);
                    break;

                case "uncheck":
                    Uncheck(commandRest[1]);
                    break;

                case "help":
                    Help();
                    break;

                default:
                    Error(command);
                    break;
            }
        }

        private void Show()
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
        }

        private void Add(string commandLine)
        {
            string[] subcommandRest = commandLine.Split(" ".ToCharArray(), 2);
            string subcommand = subcommandRest[0];

            if (subcommand == "project")
            {
                AddProject(subcommandRest[1]);
            }
            else if (subcommand == "task")
            {
                string[] projectTask = subcommandRest[1].Split(" ".ToCharArray(), 2);

                AddTask(projectTask[0], projectTask[1]);
            }
        }

        private void AddProject(string name)
        {
            _tasks[name] = new ProjectItem { Name = name, Tasks = [] };
        }

        private void AddTask(string project, string description)
        {
            if (!_tasks.TryGetValue(project, out ProjectItem projectTasks))
            {
                Console.WriteLine("Could not find a project with the name \"{0}\".", project);

                return;
            }

            projectTasks.Tasks.Add(new TaskItem { Id = NextId(), Description = description, IsDone = false });
        }

        private void Check(string idString)
        {
            SetDone(idString, true);
        }

        private void Uncheck(string idString)
        {
            SetDone(idString, false);
        }

        private void SetDone(string idString, bool isDone)
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
        }

        private void Help()
        {
            _console.WriteLine("Commands:");
            _console.WriteLine("  show");
            _console.WriteLine("  add project <project name>");
            _console.WriteLine("  add task <project name> <task description>");
            _console.WriteLine("  check <task ID>");
            _console.WriteLine("  uncheck <task ID>");
            _console.WriteLine();
        }

        private void Error(string command)
        {
            _console.WriteLine("I don't know what the command \"{0}\" is.", command);
        }

        private long NextId()
        {
            return ++_lastId;
        }
    }
}

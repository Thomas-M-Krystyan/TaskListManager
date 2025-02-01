﻿using TaskList.ConsoleApp.IO;
using TaskList.ConsoleApp.IO.Interfaces;
using TaskList.Domain.Models;

namespace TaskList.ConsoleApp
{
    internal class Program
    {
        private const string QUIT = "quit";
        public static readonly string startupText = "Welcome to TaskList! Type 'help' for available commands.";

        private readonly IDictionary<string, IList<TaskItem>> tasks = new Dictionary<string, IList<TaskItem>>();
        private readonly IConsole console;

        private long lastId = 0;

        public static void Main(string[] args)
        {
            new Program(new RealConsole()).Run();
        }

        public Program(IConsole console)
        {
            this.console = console;
        }

        public void Run()
        {
            console.WriteLine(startupText);
            while (true)
            {
                console.Write("> ");
                string command = console.ReadLine();
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
            foreach (KeyValuePair<string, IList<TaskItem>> project in tasks)
            {
                console.WriteLine(project.Key);
                foreach (TaskItem task in project.Value)
                {
                    console.WriteLine("    [{0}] {1}: {2}", task.Done ? 'x' : ' ', task.Id, task.Description);
                }

                console.WriteLine();
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
            tasks[name] = [];
        }

        private void AddTask(string project, string description)
        {
            if (!tasks.TryGetValue(project, out IList<TaskItem> projectTasks))
            {
                Console.WriteLine("Could not find a project with the name \"{0}\".", project);
                return;
            }

            projectTasks.Add(new TaskItem { Id = NextId(), Description = description, Done = false });
        }

        private void Check(string idString)
        {
            SetDone(idString, true);
        }

        private void Uncheck(string idString)
        {
            SetDone(idString, false);
        }

        private void SetDone(string idString, bool done)
        {
            int id = int.Parse(idString);
            TaskItem? identifiedTask = tasks
                .Select(project => project.Value.FirstOrDefault(task => task.Id == id))
                .Where(task => task != null)
                .FirstOrDefault();
            if (identifiedTask == null)
            {
                console.WriteLine("Could not find a task with an ID of {0}.", id);
                return;
            }

            identifiedTask.Done = done;
        }

        private void Help()
        {
            console.WriteLine("Commands:");
            console.WriteLine("  show");
            console.WriteLine("  add project <project name>");
            console.WriteLine("  add task <project name> <task description>");
            console.WriteLine("  check <task ID>");
            console.WriteLine("  uncheck <task ID>");
            console.WriteLine();
        }

        private void Error(string command)
        {
            console.WriteLine("I don't know what the command \"{0}\" is.", command);
        }

        private long NextId()
        {
            return ++lastId;
        }
    }
}

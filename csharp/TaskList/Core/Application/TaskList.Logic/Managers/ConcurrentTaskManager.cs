using System.Text.Json;
using TaskList.Domain.Models;
using TaskList.Logic.Helpers.Interfaces;
using TaskList.Logic.Managers.Interfaces;
using TaskList.Logic.Responses;

namespace TaskList.Logic.Managers
{
    /// <inheritdoc cref="ITaskManager"/>
    /// <remarks>The application generic implementation.</remarks>
    public abstract class ConcurrentTaskManager : ITaskManager
    {
        private static readonly object transactionPadlock = new();

        private readonly Dictionary<string, ProjectItem> _projects = [];
        private readonly Dictionary<long, TaskItem> _tasks = [];

        private readonly ICounterRegister _counter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentTaskManager"/> class.
        /// </summary>
        protected ConcurrentTaskManager(ICounterRegister counter)
        {
            _counter = counter;
        }

        // NOTE: It might look silly, but it's a shorter solution to make a deep-copy of the collection
        //       which results in a thread-safe and "truly" immutable (the collection itself and it's
        //       elements) snapshot of the actual task list. This approach would protect the internal
        //       data source from direct manipulations from outside and ensure the data consistency
        
        /// <inheritdoc cref="ITaskManager.GetAllProjects()"/>
        public IReadOnlyDictionary<string, ProjectItem> GetAllProjects()
        {
            lock (transactionPadlock)
            {
                return JsonSerializer.Deserialize<Dictionary<string, ProjectItem>>(
                    JsonSerializer.Serialize(_projects)) ?? [];
            }
        }

        /// <inheritdoc cref="ITaskManager.GetAllTasks()"/>
        public IReadOnlyDictionary<long, TaskItem> GetAllTasks()
        {
            lock (transactionPadlock)
            {
                return JsonSerializer.Deserialize<Dictionary<long, TaskItem>>(
                    JsonSerializer.Serialize(_tasks)) ?? [];
            }
        }

        /// <inheritdoc cref="ITaskManager.DisplayAllTasks()"/>
        public abstract CommandResponse DisplayAllTasks();

        /// <inheritdoc cref="ITaskManager.DisplayTodayTasks()"/>
        public abstract CommandResponse DisplayTodayTasks();

        /// <inheritdoc cref="ITaskManager.DisplayTasksByDeadline()"/>
        public abstract CommandResponse DisplayTasksByDeadline();

        /// <inheritdoc cref="ITaskManager.AddProject(string)"/>
        public CommandResponse AddProject(string projectName)
        {
            lock (transactionPadlock)
            {
                try
                {
                    // Add a new project
                    bool isSuccess = _projects.TryAdd(projectName,
                        new ProjectItem(projectName, _counter.GetNextProjectId()));

                    return isSuccess
                        ? CommandResponse.Success(content: string.Format("The project with name \"{0}\" was created", projectName))
                        : CommandResponse.Failure("Project with the same name already exists");
                }
                catch (Exception exception)
                {
                    return CommandResponse.Failure(exception);
                }
            }
        }

        /// <inheritdoc cref="ITaskManager.AddTask(string, string)"/>
        public CommandResponse AddTask(string projectName, string taskName)
        {
            lock (transactionPadlock)
            {
                try
                {
                    if (_projects.TryGetValue(projectName, out ProjectItem project))
                    {
                        long newTaskId = _counter.GetNextTaskId();

                        // Add a new task
                        _tasks[newTaskId]  = new TaskItem(newTaskId, project.Name, taskName);

                        // Create mapping between Project [PK] and Task [PK]
                        _projects[project.Name].TaskIds.Add(newTaskId);

                        return CommandResponse.Success(content: string.Format("The task with name \"{0}\" was added to the project \"{1}\"", taskName, projectName));
                    }
                    else
                    {
                        return CommandResponse.Failure(string.Format("Could not find a project with the name \"{0}\"", projectName));
                    }
                }
                catch (Exception exception)
                {
                    return CommandResponse.Failure(exception);
                }
            }
        }

        /// <inheritdoc cref="ITaskManager.CheckTask(long, bool)"/>
        public CommandResponse CheckTask(long taskId, bool isDone)
        {
            lock (transactionPadlock)
            {
                try
                {
                    // Determine the name of the related project
                    if (_tasks.TryGetValue(taskId, out TaskItem task))
                    {
                        // Find the task by ID
                        TaskItem existingTask = _tasks[taskId];

                        // Change task status
                        existingTask.IsDone = isDone;

                        // Update the task
                        _tasks[taskId] = existingTask;

                        return CommandResponse.Success(content: string.Format("The task with ID {0} was marked as {1}", taskId, isDone ? "finished" : "unfinished"));
                    }

                    return CommandResponse.Failure(string.Format("Could not find a task with an ID of {0}", taskId));
                }
                catch (Exception exception)
                {
                    return CommandResponse.Failure(exception);
                }
            }
        }

        /// <inheritdoc cref="ITaskManager.SetDeadline(long, DateOnly)"/>
        public CommandResponse SetDeadline(long taskId, DateOnly deadline)
        {
            lock (transactionPadlock)
            {
                try
                {
                    // Determine the name of the related project
                    if (_tasks.TryGetValue(taskId, out TaskItem task))
                    {
                        // Find the task by ID
                        TaskItem existingTask = _tasks[taskId];

                        // Change task status
                        existingTask.Deadline = deadline;

                        // Update the task
                        _tasks[taskId] = existingTask;

                        return CommandResponse.Success(content: string.Format("The deadline for the task with ID {0} was set to {1}", taskId, deadline));
                    }

                    return CommandResponse.Failure(string.Format("Could not find a task with an ID of {0}", taskId));
                }
                catch (Exception exception)
                {
                    return CommandResponse.Failure(exception);
                }
            }
        }
    }
}

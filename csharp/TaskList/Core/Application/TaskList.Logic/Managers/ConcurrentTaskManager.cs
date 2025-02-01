using System.Collections.Concurrent;
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
        private static readonly ConcurrentDictionary<string, ProjectItem> _taskList = [];

        private readonly ICounterRegister _counter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentTaskManager"/> class.
        /// </summary>
        protected ConcurrentTaskManager(ICounterRegister counter)
        {
            _counter = counter;
        }

        /// <inheritdoc cref="ITaskManager.DisplayTaskList()"/>
        public IReadOnlyDictionary<string, ProjectItem> GetTaskList()
        {
            // NOTE: It might look silly, but it's a shorter solution to make a deep-copy of the collection
            //       which results in a thread-safe and "truly" immutable (the collection itself and it's
            //       elements) snapshot of the actual task list. This approach would protect the internal
            //       data source from direct manipulations from outside and ensure the data consistency
            string serializedTaskList = JsonSerializer.Serialize(_taskList);

            return JsonSerializer.Deserialize<Dictionary<string, ProjectItem>>(serializedTaskList)
                ?? [];
        }

        /// <inheritdoc cref="ITaskManager.DisplayTaskList()"/>
        public abstract CommandResponse DisplayTaskList();

        /// <inheritdoc cref="ITaskManager.AddProject(string)"/>
        public CommandResponse AddProject(string projectName)
        {
            try
            {
                bool isSuccess = _taskList.TryAdd(projectName, new ProjectItem
                {
                    Name = projectName,
                    Tasks = []
                });

                return isSuccess
                    ? CommandResponse.Success(content: string.Format("The project with name \"{0}\" was created", projectName))
                    : CommandResponse.Failure("Project with the same name already exists");
            }
            catch (Exception exception)
            {
                return CommandResponse.Failure(exception);
            }
        }

        /// <inheritdoc cref="ITaskManager.AddTask(string, string)"/>
        public CommandResponse AddTask(string projectName, string taskName)
        {
            try
            {
                if (_taskList.TryGetValue(projectName, out ProjectItem project))
                {
                    project.Tasks.Add(new TaskItem { Id = _counter.GetNextTaskId(), Description = taskName, IsDone = false });

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

        /// <inheritdoc cref="ITaskManager.CheckTask(long, bool)"/>
        public CommandResponse CheckTask(long taskId, bool isDone)
        {
            try
            {
                var existing = _taskList
                        .Select(project => project.Value.Tasks.FirstOrDefault(task => task.Id == taskId))
                        .Where(task => task != null)
                        .FirstOrDefault();

                if (existing == null)
                {
                    return CommandResponse.Failure(string.Format("Could not find a task with an ID of {0}", taskId));
                }

                return CommandResponse.Failure(string.Format("Could not find a task with an ID of {0}", taskId));
            }
            catch (Exception exception)
            {
                return CommandResponse.Failure(exception);
            }
        }

        /// <summary>
        /// Clears the internal list of tasks.
        /// </summary>
        internal static void Reset()
        {
            _taskList.Clear();
        }
    }
}

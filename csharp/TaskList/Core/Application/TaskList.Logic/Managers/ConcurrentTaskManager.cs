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
        private static readonly ConcurrentDictionary<long, string> _primaryKeysMap = [];

        private readonly ICounterRegister _counter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentTaskManager"/> class.
        /// </summary>
        protected ConcurrentTaskManager(ICounterRegister counter)
        {
            _counter = counter;
        }

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
                    Id = _counter.GetNextProjectId(),
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
                    //project.Tasks.Add(new TaskItem(_counter.GetNextTaskId(), taskName));
                    long newTaskId = _counter.GetNextTaskId();

                    // IDs is unique and project is existing, so these operations are safe
                    project.Tasks[newTaskId] = new TaskItem
                    {
                        Id = newTaskId,
                        Description = taskName,
                        IsDone = false
                    };
                    _primaryKeysMap[newTaskId] = projectName;  // Task ID => Project Name

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
                // Determine the name of the related project
                if (_primaryKeysMap.TryGetValue(taskId, out string? relatedProjectName))
                {
                    TaskItem existing = _taskList[relatedProjectName].Tasks[taskId];

                    existing.IsDone = isDone;

                    // The collection item is struct. Modifying it "by reference" is not possible
                    _ = _taskList[relatedProjectName].Tasks.Remove(taskId);
                    _ = _taskList[relatedProjectName].Tasks[existing.Id] = existing;

                    return CommandResponse.Success(content: string.Format("The task with ID {0} was marked as {1}", taskId, isDone ? "finished" : "unfinished"));
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
            _primaryKeysMap.Clear();
        }
    }
}

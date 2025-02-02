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
        private readonly ConcurrentDictionary<string, ProjectItem> _taskList = [];
        private readonly ConcurrentDictionary<long, string> _primaryKeysMap = [];

        private readonly ICounterRegister _counter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentTaskManager"/> class.
        /// </summary>
        protected ConcurrentTaskManager(ICounterRegister counter)
        {
            _counter = counter;
        }

        /// <inheritdoc cref="ITaskManager.GetTaskList()"/>
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

        /// <inheritdoc cref="ITaskManager.DisplayAllTasks()"/>
        public abstract CommandResponse DisplayAllTasks();

        /// <inheritdoc cref="ITaskManager.DisplayTodayTasks()"/>
        public abstract CommandResponse DisplayTodayTasks();

        /// <inheritdoc cref="ITaskManager.AddProject(string)"/>
        public CommandResponse AddProject(string projectName)
        {
            try
            {
                // Add a new project to the task list
                bool isSuccess = _taskList.TryAdd(projectName, new ProjectItem(_counter.GetNextProjectId(), projectName));

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
                    // Create new task and add it to the project
                    long newTaskId = _counter.GetNextTaskId();
                    project.Tasks[newTaskId] = new TaskItem(newTaskId, taskName);

                    // Create mapping between Task ID (PK) and Project Name (PK)
                    _primaryKeysMap[newTaskId] = projectName;

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
                    // Find the task by ID
                    TaskItem existingTask = _taskList[relatedProjectName].Tasks[taskId];

                    // Update the task status
                    existingTask.IsDone = isDone;
                    UpdateTask(relatedProjectName, existingTask);

                    return CommandResponse.Success(content: string.Format("The task with ID {0} was marked as {1}", taskId, isDone ? "finished" : "unfinished"));
                }

                return CommandResponse.Failure(string.Format("Could not find a task with an ID of {0}", taskId));
            }
            catch (Exception exception)
            {
                return CommandResponse.Failure(exception);
            }
        }

        /// <inheritdoc cref="ITaskManager.SetDeadline(long, DateOnly)"/>
        public CommandResponse SetDeadline(long taskId, DateOnly deadline)
        {
            try
            {
                // Determine the name of the related project
                if (_primaryKeysMap.TryGetValue(taskId, out string? relatedProjectName))
                {
                    // Find the task by ID
                    TaskItem existingTask = _taskList[relatedProjectName].Tasks[taskId];

                    // Update the task status
                    existingTask.Deadline = deadline;
                    UpdateTask(relatedProjectName, existingTask);

                    return CommandResponse.Success(content: string.Format("The deadline for the task with ID {0} was set to {1}", taskId, deadline));
                }

                return CommandResponse.Failure(string.Format("Could not find a task with an ID of {0}", taskId));
            }
            catch (Exception exception)
            {
                return CommandResponse.Failure(exception);
            }
        }

        #region Helper methods
        private void UpdateTask(string projectName, TaskItem task)
        {
            // The collection item is struct. Modifying it "by reference" is not possible
            _ = _taskList[projectName].Tasks.Remove(task.Id);
            _ = _taskList[projectName].Tasks[task.Id] = task;
        }
        #endregion
    }
}

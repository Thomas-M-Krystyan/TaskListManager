using System.Collections.Concurrent;
using System.Text.Json;
using TaskList.Domain.Models;
using TaskList.Logic.Managers.Interfaces;
using TaskList.Logic.Responses;

namespace TaskList.Logic.Managers
{
    /// <inheritdoc cref="ITaskManager"/>
    /// <remarks>The application generic implementation.</remarks>
    public abstract class ConcurrentTaskManager : ITaskManager
    {
        internal readonly ConcurrentDictionary<string, ProjectItem> TaskList = [];

        public IReadOnlyDictionary<string, ProjectItem> GetTaskList()
        {
            // NOTE: It might look silly, but it's a shorter solution to make a deep-copy of the collection
            //       which results in a thread-safe and "truly" immutable (the collection itself and it's
            //       elements) snapshot of the actual task list. This approach would protect the internal
            //       data source from direct manipulations from outside and ensure the data consistency
            string serializedTaskList = JsonSerializer.Serialize(TaskList);

            return JsonSerializer.Deserialize<Dictionary<string, ProjectItem>>(serializedTaskList)
                ?? [];
        }

        /// <inheritdoc cref="ITaskManager.AddProject(string)"/>
        public CommandResponse AddProject(string projectName)
        {
            try
            {
                bool isSuccess = TaskList.TryAdd(projectName, new ProjectItem
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
    }
}

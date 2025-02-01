using System.Collections.Concurrent;
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

using System.Collections.Concurrent;
using TaskList.Domain.Models;
using TaskList.Logic.Managers.Interfaces;

namespace TaskList.Logic.Managers
{
    /// <inheritdoc cref="ITaskManager"/>
    /// <remarks>The application generic implementation.</remarks>
    public abstract class ConcurrentTaskManager : ITaskManager
    {
        internal readonly ConcurrentDictionary<string, ProjectItem> TaskList = [];

        /// <inheritdoc cref="ITaskManager.AddProject(string)"/>
        public bool AddProject(string projectName)
        {
            try
            {
                bool isSuccess = TaskList.TryAdd(projectName, new ProjectItem
                {
                    Name = projectName,
                    Tasks = []
                });

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}

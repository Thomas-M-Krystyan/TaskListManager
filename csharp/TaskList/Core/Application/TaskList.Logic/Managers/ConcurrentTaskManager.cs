using System.Collections.Concurrent;
using TaskList.Domain.Models;
using TaskList.Logic.Managers.Interfaces;

namespace TaskList.Logic.Managers
{
    /// <inheritdoc cref="ITaskManager"/>
    /// <remarks>The application generic implementation.</remarks>
    public abstract class ConcurrentTaskManager : ITaskManager
    {
        private static readonly ConcurrentDictionary<string, ProjectItem> _taskList = [];

        /// <inheritdoc cref="ITaskManager.AddProject(string)"/>
        public bool AddProject(string projectName)
        {
            try
            {
                bool isSuccess = _taskList.TryAdd(projectName, new ProjectItem
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

using TaskList.Domain.Models;
using TaskList.Logic.Responses;

namespace TaskList.Logic.Managers.Interfaces
{
    /// <summary>
    /// The task manager allowing different operations on task list.
    /// </summary>
    public interface ITaskManager
    {
        /// <summary>
        /// Gets thread-safe immutable copy of the internal task list which
        /// prevents direct manipulations on the application-wide data source.
        /// </summary>
        /// <returns>
        ///   The copy of the internal task list.
        /// </returns>
        public IReadOnlyDictionary<string, ProjectItem> GetTaskList();

        /// <summary>
        /// Adds <see cref="ProjectItem"/> to the internal task list.
        /// </summary>
        /// <param name="projectName">The name of the project to add.</param>
        /// <returns>
        ///   The result of the operation.
        /// </returns>
        public CommandResponse AddProject(string projectName);
    }
}

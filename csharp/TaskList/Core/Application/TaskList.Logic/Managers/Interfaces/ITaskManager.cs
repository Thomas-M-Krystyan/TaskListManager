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
        /// Gets thread-safe immutable copy of the currently saved projects which
        /// prevents direct manipulations on the application-wide data source.
        /// </summary>
        /// <returns>
        ///   The deep-copy of a dictionary of projects.
        /// </returns>
        public IReadOnlyDictionary<string, ProjectItem> GetAllProjects();

        /// <summary>
        /// Gets thread-safe immutable copy of the currently saved task which
        /// prevents direct manipulations on the application-wide data source.
        /// </summary>
        /// <returns>
        ///   The deep-copy of a dictionary of tasks.
        /// </returns>
        public IReadOnlyDictionary<long, TaskItem> GetAllTasks();

        /// <summary>
        /// Displays all currently stored <see cref="ProjectItem"/>s and <see cref="TaskItem"/>s.
        /// </summary>
        /// <returns>
        ///   The result of the operation.
        /// </returns>
        public CommandResponse DisplayAllTasks();

        /// <summary>
        /// Displays <see cref="ProjectItem"/>s and <see cref="TaskItem"/>s with the deadline set on "today".
        /// </summary>
        /// <returns>
        ///   The result of the operation.
        /// </returns>
        public CommandResponse DisplayTodayTasks();

        /// <summary>
        /// Adds <see cref="ProjectItem"/> to the internal task list.
        /// </summary>
        /// <param name="projectName">The name of the project to add.</param>
        /// <returns>
        ///   The result of the operation.
        /// </returns>
        public CommandResponse AddProject(string projectName);

        /// <summary>
        /// Adds the <see cref="TaskItem"/> to the specific <see cref="ProjectItem"/> from the internal task list.
        /// </summary>
        /// <param name="projectName">The name of the project to add the task to.</param>
        /// <param name="taskName">The name of the task to add.</param>
        /// <returns>
        ///   The result of the operation.
        /// </returns>
        public CommandResponse AddTask(string projectName, string taskName);

        /// <summary>
        /// Marks the specific <see cref="TaskItem"/> as finished.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="isDone">Indicates whether the task is finished.</param>
        /// <returns>
        ///   The result of the operation.
        /// </returns>
        public CommandResponse CheckTask(long taskId, bool isDone);

        /// <summary>
        /// Adds the deadline to the specific <see cref="TaskItem"/>.
        /// </summary>
        /// <param name="taskId">The ID of the task.</param>
        /// <param name="deadline">The deadline of the task.</param>
        /// <returns>
        ///   The result of the operation.
        /// </returns>
        public CommandResponse SetDeadline(long taskId, DateOnly deadline);
    }
}

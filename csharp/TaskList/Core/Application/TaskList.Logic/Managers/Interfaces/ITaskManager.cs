namespace TaskList.Logic.Managers.Interfaces
{
    /// <summary>
    /// The task manager allowing different operations on task list.
    /// </summary>
    public interface ITaskManager
    {
        /// <summary>
        /// Adds <see cref="ProjectItem"/> to the internal task list.
        /// </summary>
        /// <param name="projectName">The name of the project to add.</param>
        /// <returns>
        ///   The result of the operation.
        /// </returns>
        public bool AddProject(string projectName);
    }
}

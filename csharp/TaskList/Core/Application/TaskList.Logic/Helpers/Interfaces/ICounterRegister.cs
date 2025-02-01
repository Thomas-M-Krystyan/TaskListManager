using TaskList.Domain.Models;

namespace TaskList.Logic.Helpers.Interfaces
{
    /// <summary>
    /// Controls the generation of unique IDs.
    /// </summary>
    public interface ICounterRegister
    {
        /// <summary>
        /// Generates ID for the new <see cref="TaskItem"/>.
        /// </summary>
        /// <returns>
        ///   The new, unique ID.
        /// </returns>
        public long GetNextTaskId();
    }
}

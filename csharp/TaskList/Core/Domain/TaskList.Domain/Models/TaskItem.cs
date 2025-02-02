using System.Diagnostics;
using System.Text.Json.Serialization;

namespace TaskList.Domain.Models
{
    [DebuggerDisplay("Id: {Id}, Description: {Description}, IsDone: {IsDone}")]
    public struct TaskItem
    {
        public long Id { get; }

        public string Description { get; }

        public bool IsDone { get; set; } = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskItem"/> struct.
        /// </summary>
        /// <param name="id">The unique identifier of the task.</param>
        /// <param name="description">The description of the task.</param>
        public TaskItem(long id, string description)
        {
            this.Id = id;
            this.Description = description;
        }

        /// <summary>
        /// The constructor used by the JSON converter to deserialize tasks.
        /// </summary>
        [JsonConstructor]
        internal TaskItem(long id, string description, bool isDone)
        {
            this.Id = id;
            this.Description = description;
            this.IsDone = isDone;
        }
    }
}

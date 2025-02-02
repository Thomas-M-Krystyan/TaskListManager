using System.Diagnostics;
using System.Text.Json.Serialization;

namespace TaskList.Domain.Models
{
    [DebuggerDisplay("Id: {Id}, Name: {Name}, IsDone: {IsDone}, Deadline: {Deadline}")]
    public struct TaskItem
    {
        public long Id { get; }

        public string Name { get; }

        public bool IsDone { get; set; } = false;

        public DateOnly Deadline { get; set; } = DateOnly.MaxValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskItem"/> struct.
        /// </summary>
        /// <param name="id">The unique identifier of the task.</param>
        /// <param name="name">The name of the task.</param>
        public TaskItem(long id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        /// <summary>
        /// The constructor used by the JSON converter to deserialize tasks.
        /// </summary>
        /// <param name="id">The unique identifier of the task.</param>
        /// <param name="name">The name of the task.</param>
        /// <param name="isDone">The status of the task.</param>
        /// <param name="deadline">The deadline of the task.</param>
        [JsonConstructor]
        internal TaskItem(long id, string name, bool isDone, DateOnly deadline)
        {
            this.Id = id;
            this.Name = name;
            this.IsDone = isDone;
            this.Deadline = deadline;
        }
    }
}

using System.Diagnostics;
using System.Text.Json.Serialization;

namespace TaskList.Domain.Models
{
    [DebuggerDisplay("Id: {Id}, ProjectName: {ProjectName}, Name: {Name}, IsDone: {IsDone}, Deadline: {Deadline}")]
    public struct TaskItem
    {
        // [Primary key]
        /// <summary>
        /// The ID of the task.
        /// </summary>
        public long Id { get; }

        // [Foreign key]
        /// <summary>
        /// The ID of the project with which the task is associated.
        /// </summary>
        public string ProjectName { get; }

        /// <summary>
        /// The name of the task.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The status of the task.
        /// </summary>
        public bool IsDone { get; set; } = false;

        /// <summary>
        /// The deadline of the task.
        /// </summary>
        public DateOnly Deadline { get; set; } = DateOnly.MaxValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskItem"/> struct.
        /// </summary>
        public TaskItem(long id, string projectName, string name)
        {
            this.Id = id;
            this.ProjectName = projectName;
            this.Name = name;
        }

        /// <summary>
        /// The constructor used by the JSON converter to deserialize tasks.
        /// </summary>
        [JsonConstructor]
        internal TaskItem(long id, string projectName, string name, bool isDone, DateOnly deadline)
            : this(id, projectName, name)
        {
            this.IsDone = isDone;
            this.Deadline = deadline;
        }
    }
}

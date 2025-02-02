using System.Diagnostics;
using System.Text.Json.Serialization;

namespace TaskList.Domain.Models
{
    [DebuggerDisplay("Id: {Id}, Name: {Name}, Tasks: {Tasks.Count}")]
    public readonly struct ProjectItem
    {
        public long Id { get; }

        public string Name { get; }

        public Dictionary<long, TaskItem> Tasks { get; } = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectItem"/> struct.
        /// </summary>
        /// <param name="id">The unique identifier of the project.</param>
        /// <param name="name">The name of the project.</param>
        public ProjectItem(long id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        /// <summary>
        /// The constructor used by the JSON converter to deserialize projects.
        /// </summary>
        [JsonConstructor]
        internal ProjectItem(long id, string name, Dictionary<long, TaskItem> tasks)
        {
            this.Id = id;
            this.Name = name;
            this.Tasks = tasks;
        }
    }
}

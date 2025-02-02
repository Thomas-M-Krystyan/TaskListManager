using System.Diagnostics;
using System.Text.Json.Serialization;

namespace TaskList.Domain.Models
{
    [DebuggerDisplay("Name: {Name}, TaskIds: {TaskIds.Count}, OrderNumber: {OrderNumber}")]
    public readonly struct ProjectItem
    {
        // [Primary key]
        /// <summary>
        /// The ID of the project, which is also the name of the project.
        /// </summary>
        public string Name { get; }

        // [Foreign keys]
        /// <summary>
        /// The IDs of the tasks associated with this project.
        /// </summary>
        public List<long> TaskIds { get; } = [];

        /// <summary>
        /// The order number of the project.
        /// </summary>
        public long OrderNumber { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectItem"/> struct.
        /// </summary>
        public ProjectItem(string name, long orderNumber)
        {
            this.Name = name;
            this.OrderNumber = orderNumber;
        }

        /// <summary>
        /// The constructor used by the JSON converter to deserialize projects.
        /// </summary>
        [JsonConstructor]
        internal ProjectItem(string name, long orderNumber, List<long> taskIds)
            : this(name, orderNumber)
        {
            this.TaskIds = taskIds;
        }
    }
}

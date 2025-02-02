using System.Diagnostics;

namespace TaskList.Domain.Models
{
    [DebuggerDisplay("Id: {Id}, Name: {Name}, Tasks: {Tasks.Count}")]
    public struct ProjectItem
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public Dictionary<long, TaskItem> Tasks { get; set; }
    }
}

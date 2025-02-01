using System.Diagnostics;

namespace TaskList.Domain.Models
{
    [DebuggerDisplay("Name: {Name}, Tasks: {Tasks.Count}")]
    public struct ProjectItem
    {
        public string Name { get; set; }

        public List<TaskItem> Tasks { get; set; }
    }
}

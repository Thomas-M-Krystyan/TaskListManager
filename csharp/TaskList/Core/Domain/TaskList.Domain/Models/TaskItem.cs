using System.Diagnostics;

namespace TaskList.Domain.Models
{
    [DebuggerDisplay("Id: {Id}, Description: {Description}, IsDone: {IsDone}")]
    public struct TaskItem
    {
        public long Id { get; set; }

        public string Description { get; set; }

        public bool IsDone { get; set; }
    }
}

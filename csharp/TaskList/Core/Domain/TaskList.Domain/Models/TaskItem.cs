using System.Diagnostics;

namespace TaskList.Domain.Models
{
    [DebuggerDisplay("Id: {Id}, Description: {Description}, IsDone: {IsDone}")]
    public class TaskItem
    {
        public long Id { get; set; }

        public string Description { get; set; }

        public bool Done { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc;

namespace TaskList
{
    [ApiController]
    [Route("tasks")]
    public class TasksController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<string> GetTasks()
        {
            var tasks = Enumerable.Range(1, 5).Select(index => "task" + index).ToArray();
            return tasks;
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using TaskList.WebApi.Managers.Interfaces;

namespace TaskList.WebApi.Controllers.v1
{
    /// <summary>
    /// The API controller to manage task list.
    /// </summary>
    [ApiController]
    // Versioning
    [Route("api/v1/[controller]")]
    // Data contracts
    [Consumes("application/json")]
    [Produces("application/json")]
    // Swagger UI
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    public sealed class TaskListController : ControllerBase
    {
        private readonly IWebApiTaskManager _taskManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskListController"/> class.
        /// </summary>
        /// <param name="taskManager">The task manager service.</param>
        public TaskListController(IWebApiTaskManager taskManager)
        {
            _taskManager = taskManager;
        }
    }
}

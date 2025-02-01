using Microsoft.AspNetCore.Mvc;

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
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskListController"/> class.
        /// </summary>
        public TaskListController()
        {
        }
    }
}
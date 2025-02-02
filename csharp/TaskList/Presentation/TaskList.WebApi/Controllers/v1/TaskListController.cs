using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TaskList.Logic.Responses;
using TaskList.WebApi.Managers.Interfaces;

namespace TaskList.WebApi.Controllers.v1
{
    /// <summary>
    /// The API controller to manage task list.
    /// </summary>
    [ApiController]
    // Versioning
    [Route("api/v1/tasklist")]
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

        /// <summary>
        /// Displays the list of projects and tasks.
        /// </summary>
        [HttpGet]
        [Route("tasks")]
        public async Task<IActionResult> DisplayTaskListAsync()
        {
            CommandResponse response = await Task.Run(_taskManager.DisplayTaskList);

            return response.IsSuccess
                ? Ok(response.Content)
                : BadRequest(response.Content);
        }

        /// <summary>
        /// Adds a new project.
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        [HttpPost]
        [Route("projects")]
        public async Task<IActionResult> AddProjectAsync(
            [Required, FromQuery] string projectName)
        {
            CommandResponse response = await Task.Run(() => _taskManager.AddProject(projectName));

            return response.IsSuccess
                ? Ok(response.Content)
                : BadRequest(response.Content);
        }

        /// <summary>
        /// Adds a new task to the project.
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        /// <param name="taskName">Name of the task.</param>
        [HttpPost]
        [Route("projects/{projectName}/tasks")]
        public async Task<IActionResult> AddTaskAsync(
            [Required, FromRoute] string projectName,
            [Required, FromQuery] string taskName)
        {
            CommandResponse response = await Task.Run(() => _taskManager.AddTask(projectName, taskName));

            return response.IsSuccess
                ? Ok(response.Content)
                : BadRequest(response.Content);
        }
    }
}

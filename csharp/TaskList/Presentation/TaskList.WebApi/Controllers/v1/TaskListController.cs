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
        /// <remarks>
        ///   URL: api/v1/tasklist/tasks
        /// </remarks>
        /// <returns>
        ///   The status of the operation represented by HTTP Status Code and the message.
        /// </returns>
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
        /// <param name="projectName">The name of the project.</param>
        /// <remarks>
        ///   URL: api/v1/tasklist/projects?projectName=Work
        /// </remarks>
        /// <returns>
        ///   The status of the operation represented by HTTP Status Code and the message.
        /// </returns>
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
        /// <param name="projectName">The name of the project.</param>
        /// <param name="taskName">The name of the task.</param>
        /// <remarks>
        ///   URL: api/v1/tasklist/projects/Work/tasks?taskName=Meeting
        /// </remarks>
        /// <returns>
        ///   The status of the operation represented by HTTP Status Code and the message.
        /// </returns>
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

        /// <summary>
        /// Sets the task as finished or unfinished.
        /// </summary>
        /// <param name="taskId">The unique identifier of the task.</param>
        /// <param name="isDone">The status of the task (finished or unfinished).</param>
        /// <remarks>
        ///   URL: api/v1/tasklist/tasks/1/status?isDone=true
        /// </remarks>
        /// <returns>
        ///   The status of the operation represented by HTTP Status Code and the message.
        /// </returns>
        [HttpPut]
        [Route("tasks/{taskId}/status")]
        public async Task<IActionResult> CheckTaskAsync(
            [Required, FromRoute] long taskId,
            [Required, FromQuery] bool isDone)
        {
            CommandResponse response = await Task.Run(() => _taskManager.CheckTask(taskId, isDone));

            return response.IsSuccess
                ? Ok(response.Content)
                : BadRequest(response.Content);
        }

        /// <summary>
        /// Sets the deadline for the task.
        /// </summary>
        /// <param name="taskId">The unique identifier of the task.</param>
        /// <param name="deadline">The deadline for the task.</param>
        /// <remarks>
        ///   URL: api/v1/tasklist/tasks/1?deadline=01-01-0001
        /// </remarks>
        /// <returns>
        ///   The status of the operation represented by HTTP Status Code and the message.
        /// </returns>
        [HttpPut]
        [Route("tasks/{taskId}")]
        public async Task<IActionResult> SetDeadlineAsync(
            [Required, FromRoute] long taskId,
            [Required, FromQuery] DateOnly deadline)
        {
            CommandResponse response = await Task.Run(() => _taskManager.SetDeadline(taskId, deadline));

            return response.IsSuccess
                ? Ok(response.Content)
                : BadRequest(response.Content);
        }
    }
}

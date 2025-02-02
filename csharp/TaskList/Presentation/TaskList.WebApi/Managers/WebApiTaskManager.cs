using System.Text.Json;
using TaskList.Domain.Models;
using TaskList.Logic.Helpers.Interfaces;
using TaskList.Logic.Managers;
using TaskList.Logic.Managers.Interfaces;
using TaskList.Logic.Responses;
using TaskList.WebApi.Managers.Interfaces;

namespace TaskList.WebApi.Managers
{
    /// <inheritdoc cref="IWebApiTaskManager"/>
    /// <seealso cref="ITaskManager"/>
    internal sealed class WebApiTaskManager : ConcurrentTaskManager, IWebApiTaskManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebApiTaskManager"/> class.
        /// </summary>
        public WebApiTaskManager(ICounterRegister counter) : base(counter)
        {
        }

        /// <inheritdoc cref="ITaskManager.DisplayAllTasks()"/>
        public override CommandResponse DisplayAllTasks()
        {
            try
            {
                return CommandResponse.Success(
                    JsonSerializer.Serialize(GetAllTasks()), true);
            }
            catch (Exception exception)
            {
                return CommandResponse.Failure(exception);
            }
        }

        /// <inheritdoc cref="ITaskManager.DisplayTodayTasks()"/>
        public override CommandResponse DisplayTodayTasks()
        {
            try
            {
                DateOnly today = DateOnly.FromDateTime(DateTime.Today);

                Dictionary<long, TaskItem> todayTasksSortedById = GetAllTasks()
                    // Filter by deadline (today)
                    .Where(task => task.Value.Deadline.Equals(today))
                    // Sort by ID
                    .OrderBy(task => task.Key)
                    .ToDictionary();

                return CommandResponse.Success(
                    JsonSerializer.Serialize(todayTasksSortedById), true);
            }
            catch (Exception exception)
            {
                return CommandResponse.Failure(exception);
            }
        }

        /// <inheritdoc cref="ITaskManager.DisplayTasksByDeadline()"/>
        public override CommandResponse DisplayTasksByDeadline()
        {
            try
            {
                Dictionary<long, TaskItem> tasksSortedByDeadlineProjectsAndId = GetAllTasks()
                    // Sort by deadline
                    .OrderBy(task => task.Value.Deadline)
                    // Sort by projects
                    .ThenBy(task => task.Value.ProjectName)
                    // Sort by ID
                    .ThenBy(task => task.Key)
                    .ToDictionary();

                return CommandResponse.Success(
                    JsonSerializer.Serialize(tasksSortedByDeadlineProjectsAndId), true);
            }
            catch (Exception exception)
            {
                return CommandResponse.Failure(exception);
            }
        }
    }
}

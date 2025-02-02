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

                Dictionary<long, TaskItem> todayTasks = GetAllTasks()
                    .Where(task => task.Value.Deadline.Equals(today))
                    .ToDictionary();

                return CommandResponse.Success(
                    JsonSerializer.Serialize(todayTasks), true);
            }
            catch (Exception exception)
            {
                return CommandResponse.Failure(exception);
            }
        }
    }
}

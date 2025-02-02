using System.Text.Json;
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
                    JsonSerializer.Serialize(GetTaskList()), true);
            }
            catch (Exception exception)
            {
                return CommandResponse.Failure(exception);
            }
        }

        /// <inheritdoc cref="ITaskManager.DisplayTodayTasks()"/>
        public override CommandResponse DisplayTodayTasks()
        {
            throw new NotImplementedException();
        }
    }
}

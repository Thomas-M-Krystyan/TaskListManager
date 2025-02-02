using TaskList.Logic.Helpers.Interfaces;

namespace TaskList.Logic.Helpers
{
    /// <inheritdoc cref="ICounterRegister"/>
    public sealed class CounterRegister : ICounterRegister
    {
        private static readonly object _projectPadlock = new();
        private static readonly object _taskPadlock = new();

        private long _lastProjectId = 0;
        private long _lastTaskId = 0;

        /// <inheritdoc cref="ICounterRegister.GetNextProjectId()"/>
        public long GetNextProjectId()
        {
            lock (_projectPadlock)
            {
                return ++_lastProjectId;
            }
        }

        /// <inheritdoc cref="ICounterRegister.GetNextTaskId()"/>
        public long GetNextTaskId()
        {
            lock (_taskPadlock)
            {
                return ++_lastTaskId;
            }
        }
    }
}

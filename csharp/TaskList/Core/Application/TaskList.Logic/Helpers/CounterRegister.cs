using TaskList.Logic.Helpers.Interfaces;

namespace TaskList.Logic.Helpers
{
    /// <inheritdoc cref="ICounterRegister"/>
    public sealed class CounterRegister : ICounterRegister
    {
        private static readonly object _taskPadlock = new();

        private static long _lastTaskId = 0;

        /// <inheritdoc cref="ICounterRegister.GetNextTaskId()"/>
        public long GetNextTaskId()
        {
            lock (_taskPadlock)
            {
                return ++_lastTaskId;
            }
        }

        /// <summary>
        /// Resets this shared counter.
        /// </summary>
        public static void Reset()
        {
            lock (_taskPadlock)
            {
                _lastTaskId = 0;
            }
        }
    }
}

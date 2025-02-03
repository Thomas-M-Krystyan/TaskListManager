namespace TaskList.ConsoleApp.ViewModels
{
    internal readonly struct TaskViewModel(string details)
    {
        internal string Details { get; } = details;

        /// <summary>
        /// Instruction on how the <see cref="TaskViewModel"/> should display itself.
        /// </summary>
        public override string ToString()
            => string.Format("    {0}",
                this.Details);
    }
}
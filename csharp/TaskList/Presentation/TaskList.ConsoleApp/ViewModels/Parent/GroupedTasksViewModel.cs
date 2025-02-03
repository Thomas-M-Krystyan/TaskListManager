namespace TaskList.ConsoleApp.ViewModels.Parent
{
    internal readonly struct GroupedTasksViewModel()
    {
        private static readonly string Separator = $"{Environment.NewLine}{Environment.NewLine}";

        internal Dictionary<DateOnly, DeadlineViewModel> Deadlines { get; } = [];

        public override readonly string ToString()
        {
            string aestheticDeadlines = string.Join(Separator,
                this.Deadlines.Select(deadline => deadline.Value.ToString()));

            return string.Format("{0}{1}",
                aestheticDeadlines,
                Separator);
        }
    }
}

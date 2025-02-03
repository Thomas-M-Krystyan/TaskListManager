namespace TaskList.ConsoleApp.ViewModels
{
    internal struct DeadlineViewModel(DateOnly date)
    {
        private static readonly string Separator = $"{Environment.NewLine}{Environment.NewLine}";
        
        internal DateOnly Date { get; } = date;

        internal Dictionary<string, ProjectViewModel> Projects { get; set; } = [];

        /// <summary>
        /// Instruction on how the <see cref="DeadlineViewModel"/> should display its own name and associated <see cref="ProjectViewModel"/>s.
        /// </summary>
        public override readonly string ToString()
        {
            string aestheticProjects = string.Join(Separator, 
                this.Projects.Select(project => project.Value.ToString()));

            return string.Format("{0}:{1}{2}", 
                this.Date.Equals(DateOnly.MaxValue) ? "No deadline" : this.Date.ToString("dd-MM-yyyy"),
                Environment.NewLine,
                aestheticProjects);
        }
    }
}
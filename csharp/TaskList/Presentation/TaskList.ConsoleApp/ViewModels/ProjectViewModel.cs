namespace TaskList.ConsoleApp.ViewModels
{
    internal struct ProjectViewModel(string projectName)
    {
        internal string ProjectName { get; } = projectName;

        internal List<TaskViewModel> Tasks { get; set; } = [];  // Tasks would be already filtered by deadline and project

        /// <summary>
        /// Instruction on how the <see cref="ProjectViewModel"/> should display its own name and associated <see cref="TaskViewModel"/>s.
        /// </summary>
        public override readonly string ToString()
        {
            string aestheticTasks = string.Join(Environment.NewLine,
                this.Tasks.Select(task => task.ToString()));

            return string.Format("    {0}:{1}{2}",
                this.ProjectName,
                Environment.NewLine,
                aestheticTasks);
        }
    }
}
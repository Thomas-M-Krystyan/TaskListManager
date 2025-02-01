namespace TaskList.ConsoleApp.IO.Interfaces
{
    /// <summary>
    /// The input-output interface for console application.
    /// </summary>
    public interface IConsole
    {
        /// <summary>
        /// Reads the input from the _console.
        /// </summary>
        /// <returns>
        ///   Input converted into a string.
        /// </returns>
        public string ReadLine();

        /// <summary>
        /// Writes the text to the _console.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public void Write(string format, params object[] args);

        /// <summary>
        /// Writes the text to the _console followed with a new line.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public void WriteLine(string format, params object[] args);

        /// <summary>
        /// Writes the new line to the _console.
        /// </summary>
        public void WriteLine();
    }
}

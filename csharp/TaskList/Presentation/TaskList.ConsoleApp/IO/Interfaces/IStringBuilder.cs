using System.Text;

namespace TaskList.ConsoleApp.IO.Interfaces
{
    /// <summary>
    /// Proxy interface for the <see cref="StringBuilder"/> class.
    /// </summary>
    internal interface IStringBuilder
    {
        /// <summary>
        /// Appends the given text, followed by a new line, to the underlying <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="value">The value to be appended.</param>
        public void AppendLine(string value);

        /// <summary>
        /// Appends a new line, to the underlying <see cref="StringBuilder"/>.
        /// </summary>
        public void AppendLine();

        /// <summary>
        /// Clears the underlying <see cref="StringBuilder"/>.
        /// </summary>
        public void Clear();

        /// <summary>
        /// Converts the underlying <see cref="StringBuilder"/> into a <see langword="string"/>.
        /// </summary>
        public string ToString();
    }
}

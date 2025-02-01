using System.Text;
using TaskList.ConsoleApp.IO.Interfaces;

namespace TaskList.ConsoleApp.IO
{
    /// <inheritdoc cref="IStringBuilder"/>
    internal sealed class StringBuilderProxy : IStringBuilder
    {
        private readonly StringBuilder _stringBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringBuilderProxy"/> class.
        /// </summary>
        public StringBuilderProxy()
        {
            _stringBuilder = new StringBuilder();
        }

        /// <inheritdoc cref="IStringBuilder.AppendLine(string)"/>
        public void AppendLine(string value)
        {
            _stringBuilder.AppendLine(value);
        }

        /// <inheritdoc cref="IStringBuilder.AppendLine()"/>
        public void AppendLine()
        {
            _stringBuilder.AppendLine();
        }

        /// <inheritdoc cref="IStringBuilder.Clear()"/>
        public void Clear()
        {
            _stringBuilder.Clear();
        }

        /// <inheritdoc cref="IStringBuilder.ToString()"/>
        public new string ToString()
        {
            return _stringBuilder.ToString();
        }
    }
}

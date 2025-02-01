using TaskList.ConsoleApp.IO.Interfaces;

namespace TaskList.ConsoleApp.IO
{
    /// <summary>
    /// The real input-output service for the console application.
    /// </summary>
    public class RealConsole : IConsole
    {
        /// <inheritdoc cref="IConsole.ReadLine()"/>
        public string ReadLine()
        {
            return Console.ReadLine() ?? string.Empty;
        }

        /// <inheritdoc cref="IConsole.Write(string, object[])"/>
        public void Write(string format, params object[] args)
        {
            Console.Write(format, args);
        }

        /// <inheritdoc cref="IConsole.WriteLine(string, object[])"/>
        public void WriteLine(string format, params object[] args)
        {
            Console.WriteLine(format, args);
        }

        /// <inheritdoc cref="IConsole.WriteLine()"/>
        public void WriteLine()
        {
            Console.WriteLine();
        }
    }
}

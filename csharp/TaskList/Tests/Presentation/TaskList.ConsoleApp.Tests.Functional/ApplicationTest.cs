using TaskList.ConsoleApp.Controllers;
using TaskList.ConsoleApp.Managers;
using TaskList.ConsoleApp.Tests.Functional._Utilities;
using TaskList.Logic.Helpers;

namespace TaskList.ConsoleApp.Tests.Functional
{
    [TestFixture]
    public sealed class ApplicationTest
    {
        public const string PROMPT = "> ";

        private FakeConsole _console;
        private Thread _applicationThread;

        [SetUp]
        public void StartTheApplication()
        {
            // Arrange
            _console = new FakeConsole();
            CounterRegister realCounterRegister = new();
            ConsoleTaskManager realTaskManager = new(realCounterRegister);
            TaskListController taskList = new(_console, realTaskManager);

            // Act
            _applicationThread = new Thread(taskList.Run);
            _applicationThread.Start();

            // Assert
            ReadLines(TaskListController.StartupText);
        }

        [TearDown]
        public void KillTheApplication()
        {
            if (_applicationThread == null || !_applicationThread.IsAlive)
            {
                return;
            }

            #pragma warning disable SYSLIB0006  // Type or member is obsolete
            _applicationThread.Abort();
            #pragma warning restore SYSLIB0006

            throw new Exception("The application is still running.");
        }

        [Test, Timeout(1000)]
        public void ItWorks()
        {
            Execute("show");

            Execute("add project secrets");
            Execute("add task secrets Eat more donuts.");
            Execute("add task secrets Destroy all humans.");

            Execute("show");
            ReadLines(
                "secrets",
                "    [ ] 1: Eat more donuts.",
                "    [ ] 2: Destroy all humans.",
                ""
            );

            Execute("add project training");
            Execute("add task training Four Elements of Simple Design");
            Execute("add task training SOLID");
            Execute("add task training Coupling and Cohesion");
            Execute("add task training Primitive Obsession");
            Execute("add task training Outside-In TDD");
            Execute("add task training Interaction-Driven Design");

            Execute("check 1");
            Execute("check 3");
            Execute("check 5");
            Execute("check 6");

            Execute("show");
            ReadLines(
                "secrets",
                "    [x] 1: Eat more donuts.",
                "    [ ] 2: Destroy all humans.",
                "",
                "training",
                "    [x] 3: Four Elements of Simple Design",
                "    [ ] 4: SOLID",
                "    [x] 5: Coupling and Cohesion",
                "    [x] 6: Primitive Obsession",
                "    [ ] 7: Outside-In TDD",
                "    [ ] 8: Interaction-Driven Design",
                ""
            );

            Execute("quit");
        }

        private void Execute(string command)
        {
            Read(PROMPT);
            Write(command);
        }

        private void Read(string expectedOutput)
        {
            string actualOutput = _console.RetrieveOutput(expectedOutput.Length);

            Assert.That(actualOutput, Is.EqualTo(expectedOutput));
        }

        private void ReadLines(params string[] expectedOutput)
        {
            foreach (string line in expectedOutput)
            {
                Read(line + Environment.NewLine);
            }
        }

        private void Write(string input)
        {
            _console.SendInput(input + Environment.NewLine);
        }
    }
}

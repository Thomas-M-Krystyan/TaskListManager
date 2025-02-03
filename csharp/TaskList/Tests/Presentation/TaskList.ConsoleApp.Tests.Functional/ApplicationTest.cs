using TaskList.ConsoleApp.Controllers;
using TaskList.ConsoleApp.IO;
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
            StringBuilderProxy realStringBuilder = new();
            CounterRegister realCounterRegister = new();
            ConsoleTaskManager realTaskManager = new(realStringBuilder, realCounterRegister);
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
            // Display after the start
            Execute("show");

            // Adding items
            Execute("add project Secrets");
            Execute("add task Secrets Eat more donuts.");
            Execute("add task Secrets Destroy all humans.");

            Execute("show");
            ReadLines(
                "Secrets:",
                "    [ ] 1: Eat more donuts.",
                "    [ ] 2: Destroy all humans.",
                ""
            );

            // Checking
            Execute("add project Training");
            Execute("add task Training Four Elements of Simple Design");
            Execute("add task Training SOLID");
            Execute("add task Training Coupling and Cohesion");
            Execute("add task Training Primitive Obsession");
            Execute("add task Training Outside-In TDD");
            Execute("add task Training Interaction-Driven Design");

            Execute("check 1");
            Execute("check 3");
            Execute("check 5");
            Execute("check 6");

            Execute("show");
            ReadLines(
                "Secrets:",
                "    [x] 1: Eat more donuts.",
                "    [ ] 2: Destroy all humans.",
                "",
                "Training:",
                "    [x] 3: Four Elements of Simple Design",
                "    [ ] 4: SOLID",
                "    [x] 5: Coupling and Cohesion",
                "    [x] 6: Primitive Obsession",
                "    [ ] 7: Outside-In TDD",
                "    [ ] 8: Interaction-Driven Design",
                ""
            );

            // Unchecking
            Execute("uncheck 1");

            Execute("show");
            ReadLines(
                "Secrets:",
                "    [ ] 1: Eat more donuts.",
                "    [ ] 2: Destroy all humans.",
                "",
                "Training:",
                "    [x] 3: Four Elements of Simple Design",
                "    [ ] 4: SOLID",
                "    [x] 5: Coupling and Cohesion",
                "    [x] 6: Primitive Obsession",
                "    [ ] 7: Outside-In TDD",
                "    [ ] 8: Interaction-Driven Design",
                ""
            );

            // Deadlines
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            DateOnly tomorrow = today.AddDays(1);

            Execute($"deadline 2 {today}");
            Execute($"deadline 4 {today}");
            Execute($"deadline 5 {today}");
            Execute($"deadline 8 {tomorrow}");

            Execute("today");
            ReadLines(
                "Secrets:",
                "    [ ] 2: Destroy all humans.",
                "",
                "Training:",
                "    [ ] 4: SOLID",
                "    [x] 5: Coupling and Cohesion",
                ""
            );

            string todayString = today.ToString("dd-MM-yyyy");
            string tomorrowString = tomorrow.ToString("dd-MM-yyyy");

            Execute("view-by-deadline");
            ReadLines(
                $"{todayString}:",
                "    Secrets:",
                "        [ ] 2: Destroy all humans.",
                "",
                "    Training:",
                "        [ ] 4: SOLID",
                "        [x] 5: Coupling and Cohesion",
                "",
                $"{tomorrowString}:",
                "    Training:",
                "        [ ] 8: Interaction-Driven Design",
                "",
                "No deadline:",
                "    Secrets:",
                "        [ ] 1: Eat more donuts.",
                "",
                "    Training:",
                "        [x] 3: Four Elements of Simple Design",
                "        [x] 6: Primitive Obsession",
                "        [ ] 7: Outside-In TDD",
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

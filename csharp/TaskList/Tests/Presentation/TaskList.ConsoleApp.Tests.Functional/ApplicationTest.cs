using TaskList.ConsoleApp.Tests.Functional._Utilities;

namespace TaskList.ConsoleApp.Tests.Functional
{
    [TestFixture]
    public sealed class ApplicationTest
    {
        public const string PROMPT = "> ";

        private FakeConsole console;
        private Thread applicationThread;

        [SetUp]
        public void StartTheApplication()
        {
            this.console = new FakeConsole();
            TaskList.TaskList taskList = new(console);
            this.applicationThread = new Thread(taskList.Run);
            applicationThread.Start();
            ReadLines(TaskList.TaskList.startupText);
        }

        [TearDown]
        public void KillTheApplication()
        {
            if (applicationThread == null || !applicationThread.IsAlive)
            {
                return;
            }

            applicationThread.Abort();
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
            int length = expectedOutput.Length;
            string actualOutput = console.RetrieveOutput(expectedOutput.Length);
            Assert.AreEqual(expectedOutput, actualOutput);
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
            console.SendInput(input + Environment.NewLine);
        }
    }
}

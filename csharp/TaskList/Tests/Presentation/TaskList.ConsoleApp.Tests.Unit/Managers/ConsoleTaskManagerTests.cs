using Moq;
using TaskList.ConsoleApp.IO;
using TaskList.ConsoleApp.IO.Interfaces;
using TaskList.Logic.Helpers.Interfaces;

namespace TaskList.WebApp.Tests.Unit.Managers
{
    public class ConsoleTaskManagerTests
    {
        #region Mocks
        private readonly Mock<IStringBuilder> _stringBuilderMock = new(MockBehavior.Strict);
        private readonly Mock<ICounterRegister> _counterMock = new(MockBehavior.Strict);
        #endregion

        [SetUp]
        public void SetUp()
        {
            _stringBuilderMock.Reset();
            _counterMock.Reset();
        }

        #region DisplayTaskList()
        [Test]
        public void DisplayTaskList_TaskList_Filled_ReturnsSuccess()
        {
            // Arrange
            const string projectName = "Project 1";
            const string taskName1 = "Task 1";

            _counterMock
                .Setup(mock => mock.GetNextProjectId())
                .Returns(default(long));

            _counterMock
                .Setup(mock => mock.GetNextTaskId())
                .Returns(default(long));

            var taskManager = new ConsoleTaskManager(new StringBuilderProxy(), _counterMock.Object);

            taskManager.AddProject(projectName);
            taskManager.AddTask(projectName, taskName1);

            // Act
            var result = taskManager.DisplayTaskList();

            // Assert
            string expectedOutput =
                $"{projectName}\r\n" +
                $"    [ ] 0: {taskName1}\r\n" +
                $"\r\n";

            Assert.Multiple(() =>
            {
                _counterMock.Verify(mock => mock.GetNextProjectId(), Times.Once);
                _counterMock.Verify(mock => mock.GetNextTaskId(), Times.Once);

                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.Content, Is.EqualTo(expectedOutput));
            });
        }

        [Test]
        public void DisplayTaskList_TaskList_Empty_ReturnsSuccess()
        {
            // Arrange
            var taskManager = new ConsoleTaskManager(new StringBuilderProxy(), _counterMock.Object);

            // Act
            var result = taskManager.DisplayTaskList();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.Content, Is.Empty);
            });
        }

        [Test]
        public void DisplayTaskList_StringBuilder_ThrowingException_ReturnsFailure()
        {
            // Arrange
            const string exceptionMessage = "Expected test exception";

            _stringBuilderMock
                .Setup(mock => mock.Clear())
                .Throws(new Exception(exceptionMessage));

            _counterMock
                .Setup(mock => mock.GetNextTaskId())
                .Returns(default(long));

            var taskManager = new ConsoleTaskManager(_stringBuilderMock.Object, _counterMock.Object);

            // Act
            var result = taskManager.DisplayTaskList();

            // Assert
            Assert.Multiple(() =>
            {
                _stringBuilderMock.Verify(mock => mock.Clear(), Times.Once);
                _counterMock.Verify(mock => mock.GetNextTaskId(), Times.Never);

                Assert.That(result.IsFailure, Is.True);
                Assert.That(result.Content, Is.EqualTo($"Operation failed: {exceptionMessage}."));
            });
        }
        #endregion

        #region Help()
        [Test]
        public void Help_ReturnsExpectedString()
        {
            // Arrange
            var taskManager = new ConsoleTaskManager(_stringBuilderMock.Object, _counterMock.Object);

            // Act
            var actualMessage = taskManager.Help();

            // Assert
            const string expectedMessage =
                $"Commands:\r\n" +
                "  show\r\n" +
                "  add project <project name>\r\n" +
                "  add task <project name> <task description>\r\n" +
                "  check <task ID>\r\n" +
                "  uncheck <task ID>\r\n" +
                "  quit\r\n";

            Assert.That(actualMessage, Is.EqualTo(expectedMessage));
        }
        #endregion

        #region Error()
        [Test]
        public void Error_ReturnsExpectedString()
        {
            // Arrange
            const string invalidCommand = "?:<!";

            var taskManager = new ConsoleTaskManager(_stringBuilderMock.Object, _counterMock.Object);

            // Act
            var actualMessage = taskManager.Error(invalidCommand);

            // Assert
            Assert.That(actualMessage, Is.EqualTo($"I don't know what the command \"{invalidCommand}\" is."));
        }
        #endregion
    }
}
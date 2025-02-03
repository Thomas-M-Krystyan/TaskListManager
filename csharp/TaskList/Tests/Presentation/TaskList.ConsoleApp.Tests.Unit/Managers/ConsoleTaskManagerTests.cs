using Moq;
using TaskList.ConsoleApp.IO;
using TaskList.ConsoleApp.IO.Interfaces;
using TaskList.ConsoleApp.Managers;
using TaskList.Logic.Helpers.Interfaces;
using TaskList.Logic.Responses;

namespace TaskList.ConsoleApp.Tests.Unit.Managers
{
    [TestFixture]
    public sealed class ConsoleTaskManagerTests
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

        #region DisplayAllTasks()
        [Test]
        public void DisplayAllTasks_TaskList_Empty_ReturnsSuccess()
        {
            // Arrange
            ConsoleTaskManager taskManager = new(new StringBuilderProxy(), _counterMock.Object);

            // Act
            CommandResponse result = taskManager.DisplayAllTasks();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.Content, Is.Empty);
            });
        }

        [Test]
        public void DisplayAllTasks_TaskList_Filled_ReturnsSuccess()
        {
            // Arrange
            const string projectName = "Project 1";
            const string taskName = "Task 1";

            _ = _counterMock
                .Setup(mock => mock.GetNextProjectId())
                .Returns(default(long));

            _ = _counterMock
                .Setup(mock => mock.GetNextTaskId())
                .Returns(default(long));

            ConsoleTaskManager taskManager = new(new StringBuilderProxy(), _counterMock.Object);

            _ = taskManager.AddProject(projectName);
            _ = taskManager.AddTask(projectName, taskName);

            // Act
            CommandResponse result = taskManager.DisplayAllTasks();

            // Assert
            string expectedOutput =
                $"{projectName}:\r\n" +
                $"    [ ] 0: {taskName}\r\n" +
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
        public void DisplayAllTasks_StringBuilder_ThrowingException_ReturnsFailure()
        {
            // Arrange
            const string exceptionMessage = "Expected test exception";

            _ = _stringBuilderMock
                .Setup(mock => mock.Clear())
                .Throws(new Exception(exceptionMessage));

            _ = _counterMock
                .Setup(mock => mock.GetNextTaskId())
                .Returns(default(long));

            ConsoleTaskManager taskManager = new(_stringBuilderMock.Object, _counterMock.Object);

            // Act
            CommandResponse result = taskManager.DisplayAllTasks();

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

        #region DisplayTodayTasks()
        [Test]
        public void DisplayTodayTasks_TaskList_Empty_ReturnsSuccess()
        {
            // Arrange
            ConsoleTaskManager taskManager = new(new StringBuilderProxy(), _counterMock.Object);

            // Act
            CommandResponse result = taskManager.DisplayTodayTasks();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.Content, Is.Empty);
            });
        }

        [Test]
        public void DisplayTodayTasks_TaskList_Filled_NotToday_ReturnsSuccess()
        {
            // Arrange
            const string projectName = "Project 1";
            const string taskName = "Task 1";

            _ = _counterMock
                .Setup(mock => mock.GetNextProjectId())
                .Returns(default(long));

            _ = _counterMock
                .Setup(mock => mock.GetNextTaskId())
                .Returns(default(long));

            ConsoleTaskManager taskManager = new(new StringBuilderProxy(), _counterMock.Object);

            _ = taskManager.AddProject(projectName);
            _ = taskManager.AddTask(projectName, taskName);

            // Act
            CommandResponse result = taskManager.DisplayTodayTasks();

            // Assert
            Assert.Multiple(() =>
            {
                _counterMock.Verify(mock => mock.GetNextProjectId(), Times.Once);
                _counterMock.Verify(mock => mock.GetNextTaskId(), Times.Once);

                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.Content, Is.Empty);
            });
        }

        [Test]
        public void DisplayTodayTasks_TaskList_Filled_Today_ReturnsSuccess()
        {
            // Arrange
            const string projectName = "Project 1";
            const string taskName = "Task 1";
            const long taskId = 1;

            _ = _counterMock
                .Setup(mock => mock.GetNextProjectId())
                .Returns(default(long));

            _ = _counterMock
                .Setup(mock => mock.GetNextTaskId())
                .Returns(taskId);

            ConsoleTaskManager taskManager = new(new StringBuilderProxy(), _counterMock.Object);

            _ = taskManager.AddProject(projectName);
            _ = taskManager.AddTask(projectName, taskName);
            _ = taskManager.SetDeadline(taskId, DateOnly.FromDateTime(DateTime.Today));

            // Act
            CommandResponse result = taskManager.DisplayTodayTasks();

            // Assert
            string expectedOutput =
                $"{projectName}:\r\n" +
                $"    [ ] {taskId}: {taskName}\r\n" +
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
        public void DisplayTodayTasks_StringBuilder_ThrowingException_ReturnsFailure()
        {
            // Arrange
            const string exceptionMessage = "Expected test exception";

            _ = _stringBuilderMock
                .Setup(mock => mock.Clear())
                .Throws(new Exception(exceptionMessage));

            _ = _counterMock
                .Setup(mock => mock.GetNextTaskId())
                .Returns(default(long));

            ConsoleTaskManager taskManager = new(_stringBuilderMock.Object, _counterMock.Object);

            // Act
            CommandResponse result = taskManager.DisplayTodayTasks();

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

        #region DisplayTasksByDeadline()
        [Test]
        public void DisplayTasksByDeadline_TaskList_Empty_ReturnsSuccess()
        {
            // Arrange
            ConsoleTaskManager taskManager = new(new StringBuilderProxy(), _counterMock.Object);

            // Act
            CommandResponse result = taskManager.DisplayTasksByDeadline();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.Content, Is.Empty);
            });
        }

        [Test]
        public void DisplayTasksByDeadline_TaskList_Filled_ReturnsSuccess()
        {
            // Arrange
            const string projectName = "Project 1";
            const string taskName = "Task 1";
            const long taskId = 1;
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);

            _ = _counterMock
                .Setup(mock => mock.GetNextProjectId())
                .Returns(default(long));

            _ = _counterMock
                .Setup(mock => mock.GetNextTaskId())
                .Returns(taskId);

            ConsoleTaskManager taskManager = new(new StringBuilderProxy(), _counterMock.Object);

            _ = taskManager.AddProject(projectName);
            _ = taskManager.AddTask(projectName, taskName);
            _ = taskManager.SetDeadline(taskId, today);

            // Act
            CommandResponse result = taskManager.DisplayTasksByDeadline();

            // Assert
            string expectedOutput =
                $"{today:dd-MM-yyyy}:\r\n" +
                $"    {projectName}:\r\n" +
                $"        [ ] {taskId}: {taskName}\r\n" +
                $"\r\n";

            Assert.Multiple(() =>
            {
                _counterMock.Verify(mock => mock.GetNextProjectId(), Times.Once);
                _counterMock.Verify(mock => mock.GetNextTaskId(), Times.Once);

                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.Content, Is.EqualTo(expectedOutput));
            });
        }
        #endregion

        #region Help()
        [Test]
        public void Help_ReturnsExpectedString()
        {
            // Arrange
            ConsoleTaskManager taskManager = new(_stringBuilderMock.Object, _counterMock.Object);

            // Act
            string actualMessage = taskManager.Help();

            // Assert
            const string expectedMessage =
                $"Commands:\r\n" +
                "  show\r\n" +
                "  today\r\n" +
                "  view-by-deadline\r\n" +
                "  add project <project name>\r\n" +
                "  add task <project name> <task description>\r\n" +
                "  check <task ID>\r\n" +
                "  uncheck <task ID>\r\n" +
                "  deadline <task ID> <deadline>\r\n" +
                "  help\r\n" +
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

            ConsoleTaskManager taskManager = new(_stringBuilderMock.Object, _counterMock.Object);

            // Act
            string actualMessage = taskManager.Error(invalidCommand);

            // Assert
            Assert.That(actualMessage, Is.EqualTo($"I don't know what the command \"{invalidCommand}\" is."));
        }
        #endregion
    }
}
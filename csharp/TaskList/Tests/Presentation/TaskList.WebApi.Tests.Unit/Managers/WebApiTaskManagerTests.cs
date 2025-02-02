using Moq;
using TaskList.Logic.Helpers.Interfaces;
using TaskList.Logic.Responses;
using TaskList.WebApi.Managers;

namespace TaskList.WebApi.Tests.Unit.Managers
{
    [TestFixture]
    public sealed class WebApiTaskManagerTests
    {
        #region Data
        private const string ProjectName = "Project 1";
        private const string TaskName = "Task 1";
        private const long TaskId = 1;
        private const string EmptyJson = "{}";
        #endregion

        #region Mocks
        private readonly Mock<ICounterRegister> _counterMock = new(MockBehavior.Strict);
        #endregion

        [SetUp]
        public void SetUp()
        {
            _counterMock.Reset();
        }

        #region DisplayAllTasks()
        [Test]
        public void DisplayAllTasks_TaskList_Empty_ReturnsSuccess()
        {
            // Arrange
            WebApiTaskManager taskManager = new(_counterMock.Object);

            // Act
            CommandResponse response = taskManager.DisplayAllTasks();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.IsSuccess, Is.True);
                Assert.That(response.Content, Is.EqualTo(EmptyJson));
            });
        }

        [Test]
        public void DisplayAllTasks_TaskList_Filled_ReturnsSuccess()
        {
            // Arrange
            _ = _counterMock
                .Setup(counter => counter.GetNextProjectId())
                .Returns(default(long));

            _ = _counterMock
                .Setup(counter => counter.GetNextTaskId())
                .Returns(TaskId);

            WebApiTaskManager taskManager = new(_counterMock.Object);

            _ = taskManager.AddProject(ProjectName);
            _ = taskManager.AddTask(ProjectName, TaskName);

            // Act
            CommandResponse response = taskManager.DisplayAllTasks();

            // Assert
            Assert.Multiple(() =>
            {
                _counterMock.Verify(mock => mock.GetNextProjectId(), Times.Once);
                _counterMock.Verify(mock => mock.GetNextTaskId(), Times.Once);

                string expectedJson =
                    $"{{\"{TaskId}\":" +
                      $"{{" +
                        $"\"Id\":{TaskId}," +
                        $"\"ProjectName\":\"{ProjectName}\"," +
                        $"\"Name\":\"{TaskName}\"," +
                        $"\"IsDone\":false," +
                        $"\"Deadline\":\"9999-12-31\"" +
                      $"}}" +
                    $"}}";

                Assert.That(response.IsSuccess, Is.True);
                Assert.That(response.Content, Is.EqualTo(expectedJson));
            });
        }
        #endregion

        #region DisplayTodayTasks()
        [Test]
        public void DisplayTodayTasks_TaskList_Empty_ReturnsSuccess()
        {
            // Arrange
            WebApiTaskManager taskManager = new(_counterMock.Object);

            // Act
            CommandResponse response = taskManager.DisplayTodayTasks();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.IsSuccess, Is.True);
                Assert.That(response.Content, Is.EqualTo(EmptyJson));
            });
        }

        [Test]
        public void DisplayTodayTasks_TaskList_Filled_NotToday_ReturnsSuccess()
        {
            // Arrange
            _ = _counterMock
                .Setup(counter => counter.GetNextProjectId())
                .Returns(default(long));

            _ = _counterMock
                .Setup(counter => counter.GetNextTaskId())
                .Returns(TaskId);

            WebApiTaskManager taskManager = new(_counterMock.Object);

            _ = taskManager.AddProject(ProjectName);
            _ = taskManager.AddTask(ProjectName, TaskName);

            // Act
            CommandResponse response = taskManager.DisplayTodayTasks();

            // Assert
            Assert.Multiple(() =>
            {
                _counterMock.Verify(mock => mock.GetNextProjectId(), Times.Once);
                _counterMock.Verify(mock => mock.GetNextTaskId(), Times.Once);

                Assert.That(response.IsSuccess, Is.True);
                Assert.That(response.Content, Is.EqualTo(EmptyJson));
            });
        }

        [Test]
        public void DisplayTodayTasks_TaskList_Filled_Today_ReturnsSuccess()
        {
            // Arrange
            _ = _counterMock
                .Setup(counter => counter.GetNextProjectId())
                .Returns(default(long));

            _ = _counterMock
                .Setup(counter => counter.GetNextTaskId())
                .Returns(TaskId);

            WebApiTaskManager taskManager = new(_counterMock.Object);

            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            _ = taskManager.AddProject(ProjectName);
            _ = taskManager.AddTask(ProjectName, TaskName);
            _ = taskManager.SetDeadline(TaskId, today);

            // Act
            CommandResponse response = taskManager.DisplayTodayTasks();

            // Assert
            Assert.Multiple(() =>
            {
                _counterMock.Verify(mock => mock.GetNextProjectId(), Times.Once);
                _counterMock.Verify(mock => mock.GetNextTaskId(), Times.Once);

                string expectedJson =
                    $"{{\"{TaskId}\":" +
                      $"{{" +
                        $"\"Id\":{TaskId}," +
                        $"\"ProjectName\":\"{ProjectName}\"," +
                        $"\"Name\":\"{TaskName}\"," +
                        $"\"IsDone\":false," +
                        $"\"Deadline\":\"{today:yyyy-MM-dd}\"" +
                      $"}}" +
                    $"}}";

                Assert.That(response.IsSuccess, Is.True);
                Assert.That(response.Content, Is.EqualTo(expectedJson));
            });
        }
        #endregion
    }
}
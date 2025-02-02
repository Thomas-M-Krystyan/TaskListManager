using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskList.Logic.Helpers;
using TaskList.Logic.Managers;
using TaskList.Logic.Responses;
using TaskList.WebApi.Controllers.v1;
using TaskList.WebApi.Managers;
using TaskList.WebApi.Managers.Interfaces;

namespace TaskList.WebApi.Tests.Integration.Controllers.v1
{
    [TestFixture]
    public sealed class TaskListControllerTests
    {
        #region Data
        private const string ProjectName = "Hobby";
        private const string TaskName = "Radio";
        private const long TaskId = 1;
        #endregion

        [SetUp]
        public void SetUp()
        {
            CounterRegister.Reset();
            ConcurrentTaskManager.Reset();
        }

        #region DisplayTaskListAsync()
        [Test]
        public async Task DisplayTaskListAsync_HappyPath_IntegrationTest()
        {
            // Arrange
            WebApiTaskManager taskManager = new(new CounterRegister());
            TaskListController controller = new(taskManager);

            // Act
            ObjectResult result = (ObjectResult)await controller.DisplayTaskListAsync();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Value, Is.EqualTo("{}"));
            });
        }

        [Test]
        public async Task DisplayTaskListAsync_RainyPath_IntegrationTest()
        {
            // Arrange
            const string errorMessage = "DisplayTaskList failed.";

            Mock<IWebApiTaskManager> taskManagerMock = new(MockBehavior.Strict);
            
            _ = taskManagerMock
                .Setup(mock => mock.DisplayTaskList())
                .Returns(CommandResponse.Failure(errorMessage));

            TaskListController controller = new(taskManagerMock.Object);

            // Act
            ObjectResult result = (ObjectResult)await controller.DisplayTaskListAsync();

            // Assert
            Assert.Multiple(() =>
            {
                taskManagerMock.Verify(mock => mock.DisplayTaskList(), Times.Once);

                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                Assert.That(result.Value, Is.EqualTo($"Operation failed: {errorMessage}."));
            });
        }
        #endregion

        #region AddProjectAsync()
        [Test]
        public async Task AddProjectAsync_HappyPath_IntegrationTest()
        {
            // Arrange
            WebApiTaskManager taskManager = new(new CounterRegister());
            TaskListController controller = new(taskManager);

            // Act
            ObjectResult result = (ObjectResult)await controller.AddProjectAsync(ProjectName);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Value, Is.EqualTo($"Operation succeeded: The project with name \"{ProjectName}\" was created."));
            });
        }

        [Test]
        public async Task AddProjectAsync_RainyPath_IntegrationTest()
        {
            // Arrange
            const string errorMessage = "AddProjectAsync failed.";

            Mock<IWebApiTaskManager> taskManagerMock = new(MockBehavior.Strict);

            _ = taskManagerMock
                .Setup(mock => mock.AddProject(ProjectName))
                .Returns(CommandResponse.Failure(errorMessage));

            TaskListController controller = new(taskManagerMock.Object);

            // Act
            ObjectResult result = (ObjectResult)await controller.AddProjectAsync(ProjectName);

            // Assert
            Assert.Multiple(() =>
            {
                taskManagerMock.Verify(mock => mock.AddProject(ProjectName), Times.Once);

                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                Assert.That(result.Value, Is.EqualTo($"Operation failed: {errorMessage}."));
            });
        }
        #endregion

        #region AddTaskAsync()
        [Test]
        public async Task AddTaskAsync_HappyPath_IntegrationTest()
        {
            // Arrange
            WebApiTaskManager taskManager = new(new CounterRegister());
            _ = taskManager.AddProject(ProjectName);

            TaskListController controller = new(taskManager);

            // Act
            ObjectResult result = (ObjectResult)await controller.AddTaskAsync(ProjectName, TaskName);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Value, Is.EqualTo($"Operation succeeded: The task with name \"{TaskName}\" was added to the project \"{ProjectName}\"."));
            });
        }

        [Test]
        public async Task AddTaskAsync_RainyPath_IntegrationTest()
        {
            // Arrange
            const string errorMessage = "AddTaskAsync failed.";

            Mock<IWebApiTaskManager> taskManagerMock = new(MockBehavior.Strict);

            _ = taskManagerMock
                .Setup(mock => mock.AddTask(ProjectName, TaskName))
                .Returns(CommandResponse.Failure(errorMessage));

            TaskListController controller = new(taskManagerMock.Object);

            // Act
            ObjectResult result = (ObjectResult)await controller.AddTaskAsync(ProjectName, TaskName);

            // Assert
            Assert.Multiple(() =>
            {
                taskManagerMock.Verify(mock => mock.AddTask(ProjectName, TaskName), Times.Once);

                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                Assert.That(result.Value, Is.EqualTo($"Operation failed: {errorMessage}."));
            });
        }
        #endregion

        #region CheckTaskAsync()
        [Test]
        public async Task CheckTaskAsync_HappyPath_IntegrationTest()
        {
            // Arrange
            WebApiTaskManager taskManager = new(new CounterRegister());
            _ = taskManager.AddProject(ProjectName);
            _ = taskManager.AddTask(ProjectName, TaskName);

            TaskListController controller = new(taskManager);

            // Act
            ObjectResult result = (ObjectResult)await controller.CheckTaskAsync(TaskId, true);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Value, Is.EqualTo($"Operation succeeded: The task with ID {TaskId} was marked as finished."));
            });
        }

        [Test]
        public async Task CheckTaskAsync_RainyPath_IntegrationTest()
        {
            // Arrange
            const string errorMessage = "CheckTaskAsync failed.";

            Mock<IWebApiTaskManager> taskManagerMock = new(MockBehavior.Strict);

            _ = taskManagerMock
                .Setup(mock => mock.CheckTask(TaskId, true))
                .Returns(CommandResponse.Failure(errorMessage));

            TaskListController controller = new(taskManagerMock.Object);

            // Act
            ObjectResult result = (ObjectResult)await controller.CheckTaskAsync(TaskId, true);

            // Assert
            Assert.Multiple(() =>
            {
                taskManagerMock.Verify(mock => mock.CheckTask(TaskId, true), Times.Once);

                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                Assert.That(result.Value, Is.EqualTo($"Operation failed: {errorMessage}."));
            });
        }
        #endregion
    }
}
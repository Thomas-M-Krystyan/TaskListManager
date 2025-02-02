using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskList.Logic.Helpers;
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
        public async Task DisplayTaskListAsync_Exception_IntegrationTest()
        {
            // Arrange
            string errorMessage = $"{nameof(WebApiTaskManager.DisplayTaskList)} failed.";

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
        public async Task AddProjectAsync_RainyPath_Duplicate_IntegrationTest()
        {
            // Arrange
            WebApiTaskManager taskManager = new(new CounterRegister());

            _ = taskManager.AddProject(ProjectName);

            TaskListController controller = new(taskManager);

            // Act
            ObjectResult result = (ObjectResult)await controller.AddProjectAsync(ProjectName);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                Assert.That(result.Value, Is.EqualTo($"Operation failed: Project with the same name already exists."));
            });
        }

        [Test]
        public async Task AddProjectAsync_Exception_IntegrationTest()
        {
            // Arrange
            string errorMessage = $"{nameof(WebApiTaskManager.AddProject)} failed.";

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
        public async Task AddTaskAsync_RainyPath_NoProject_IntegrationTest()
        {
            // Arrange
            WebApiTaskManager taskManager = new(new CounterRegister());
            TaskListController controller = new(taskManager);

            // Act
            ObjectResult result = (ObjectResult)await controller.AddTaskAsync(ProjectName, TaskName);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                Assert.That(result.Value, Is.EqualTo($"Operation failed: Could not find a project with the name \"{ProjectName}\"."));
            });
        }

        [Test]
        public async Task AddTaskAsync_Exception_IntegrationTest()
        {
            // Arrange
            string errorMessage = $"{nameof(WebApiTaskManager.AddTask)} failed.";

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
        public async Task CheckTaskAsync_HappyPath_Finished_IntegrationTest()
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
        public async Task CheckTaskAsync_HappyPath_Unfinished_IntegrationTest()
        {
            // Arrange
            WebApiTaskManager taskManager = new(new CounterRegister());

            _ = taskManager.AddProject(ProjectName);
            _ = taskManager.AddTask(ProjectName, TaskName);
            _ = taskManager.CheckTask(
                taskManager.GetTaskList().First().Value.Id, true);

            TaskListController controller = new(taskManager);

            // Act
            ObjectResult result = (ObjectResult)await controller.CheckTaskAsync(TaskId, false);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Value, Is.EqualTo($"Operation succeeded: The task with ID {TaskId} was marked as unfinished."));
            });
        }

        [Test]
        public async Task CheckTaskAsync_RainyPath_NoProject_IntegrationTest()
        {
            // Arrange
            WebApiTaskManager taskManager = new(new CounterRegister());
            TaskListController controller = new(taskManager);

            // Act
            ObjectResult result = (ObjectResult)await controller.CheckTaskAsync(TaskId, true);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                Assert.That(result.Value, Is.EqualTo($"Operation failed: Could not find a task with an ID of {TaskId}."));
            });
        }

        [Test]
        public async Task CheckTaskAsync_RainyPath_NoTask_IntegrationTest()
        {
            // Arrange
            WebApiTaskManager taskManager = new(new CounterRegister());

            _ = taskManager.AddProject(ProjectName);

            TaskListController controller = new(taskManager);

            // Act
            ObjectResult result = (ObjectResult)await controller.CheckTaskAsync(TaskId, true);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                Assert.That(result.Value, Is.EqualTo($"Operation failed: Could not find a task with an ID of {TaskId}."));
            });
        }

        [Test]
        public async Task CheckTaskAsync_Exception_IntegrationTest()
        {
            // Arrange
            string errorMessage = $"{nameof(WebApiTaskManager.CheckTask)} failed.";

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

        #region SetDeadlineAsync()
        [Test]
        public async Task SetDeadlineAsync_HappyPath_IntegrationTest()
        {
            // Arrange
            WebApiTaskManager taskManager = new(new CounterRegister());

            _ = taskManager.AddProject(ProjectName);
            _ = taskManager.AddTask(ProjectName, TaskName);

            TaskListController controller = new(taskManager);

            // Act
            ObjectResult result = (ObjectResult)await controller.SetDeadlineAsync(TaskId, DateOnly.MinValue);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));
                Assert.That(result.Value, Is.EqualTo($"Operation succeeded: The deadline for the task with ID {TaskId} was set to 01.01.0001."));
            });
        }

        [Test]
        public async Task SetDeadlineAsync_RainyPath_NoProject_IntegrationTest()
        {
            // Arrange
            WebApiTaskManager taskManager = new(new CounterRegister());
            TaskListController controller = new(taskManager);

            // Act
            ObjectResult result = (ObjectResult)await controller.SetDeadlineAsync(TaskId, DateOnly.MinValue);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                Assert.That(result.Value, Is.EqualTo($"Operation failed: Could not find a task with an ID of {TaskId}."));
            });
        }

        [Test]
        public async Task SetDeadlineAsync_RainyPath_NoTask_IntegrationTest()
        {
            // Arrange
            WebApiTaskManager taskManager = new(new CounterRegister());

            _ = taskManager.AddProject(ProjectName);

            TaskListController controller = new(taskManager);

            // Act
            ObjectResult result = (ObjectResult)await controller.SetDeadlineAsync(TaskId, DateOnly.MinValue);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                Assert.That(result.Value, Is.EqualTo($"Operation failed: Could not find a task with an ID of {TaskId}."));
            });
        }

        [Test]
        public async Task SetDeadlineAsync_Exception_IntegrationTest()
        {
            // Arrange
            string errorMessage = $"{nameof(WebApiTaskManager.SetDeadline)} failed.";

            Mock<IWebApiTaskManager> taskManagerMock = new(MockBehavior.Strict);

            _ = taskManagerMock
                .Setup(mock => mock.SetDeadline(TaskId, DateOnly.MinValue))
                .Returns(CommandResponse.Failure(errorMessage));

            TaskListController controller = new(taskManagerMock.Object);

            // Act
            ObjectResult result = (ObjectResult)await controller.SetDeadlineAsync(TaskId, DateOnly.MinValue);

            // Assert
            Assert.Multiple(() =>
            {
                taskManagerMock.Verify(mock => mock.SetDeadline(TaskId, DateOnly.MinValue), Times.Once);

                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                Assert.That(result.Value, Is.EqualTo($"Operation failed: {errorMessage}."));
            });
        }
        #endregion
    }
}
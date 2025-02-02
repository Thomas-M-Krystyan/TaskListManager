using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskList.Logic.Helpers;
using TaskList.Logic.Helpers.Interfaces;
using TaskList.Logic.Responses;
using TaskList.WebApi.Controllers.v1;
using TaskList.WebApi.Managers;
using TaskList.WebApi.Managers.Interfaces;

namespace TaskList.WebApi.Tests.Integration.Controllers.v1
{
    [TestFixture]
    public sealed class TaskListControllerTests
    {
        [SetUp]
        public void Setup()
        {
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
                Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
                Assert.That(result.Value, Is.EqualTo($"Operation failed: {errorMessage}."));
            });
        }
        #endregion
    }
}
using Moq;
using TaskList.Logic.Helpers.Interfaces;
using TaskList.Logic.Responses;
using TaskList.WebApi.Managers;

namespace TaskList.WebApi.Tests.Unit.Managers
{
    [TestFixture]
    public sealed class WebApiTaskManagerTests
    {
        #region Mocks
        private readonly Mock<ICounterRegister> _counterMock = new(MockBehavior.Strict);
        #endregion

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
                Assert.That(response.Content, Is.EqualTo("{}"));
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
                .Returns(default(long));

            const string projectName = "Project 1";
            const string taskName = "Task 1";

            WebApiTaskManager taskManager = new(_counterMock.Object);

            _ = taskManager.AddProject(projectName);
            _ = taskManager.AddTask(projectName, taskName);

            // Act
            CommandResponse response = taskManager.DisplayAllTasks();

            // Assert
            Assert.Multiple(() =>
            {
                _counterMock.Verify(mock => mock.GetNextProjectId(), Times.Once);
                _counterMock.Verify(mock => mock.GetNextTaskId(), Times.Once);

                const string expectedJson =
                "{" +
                  "\"Project 1\":{" +
                    "\"Id\":0," +
                    "\"Name\":\"Project 1\"," +
                    "\"Tasks\":{" +
                      "\"0\":{" +
                        "\"Id\":0,\"Name\":\"Task 1\",\"IsDone\":false,\"Deadline\":\"9999-12-31\"" +
                      "}" +
                    "}" +
                  "}" +
                "}";

                Assert.That(response.IsSuccess, Is.True);
                Assert.That(response.Content, Is.EqualTo(expectedJson));
            });
        }
        #endregion
    }
}
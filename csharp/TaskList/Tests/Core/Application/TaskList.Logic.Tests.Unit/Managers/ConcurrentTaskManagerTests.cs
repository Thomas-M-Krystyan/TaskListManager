using Moq;
using TaskList.Domain.Models;
using TaskList.Logic.Helpers.Interfaces;
using TaskList.Logic.Managers;
using TaskList.Logic.Responses;

namespace TaskList.Logic.Tests.Unit.Managers
{
    [TestFixture]
    public sealed class ConcurrentTaskManagerTests
    {
        #region Mocks
        private readonly Mock<ICounterRegister> _counterMock = new(MockBehavior.Strict);
        #endregion

        #region Data
        private const string ProjectName = "Work";
        private const string TaskName = "Task";

        private sealed class TestTaskManager(ICounterRegister counter) : ConcurrentTaskManager(counter)
        {
            public override CommandResponse DisplayTaskList()
            {
                throw new NotImplementedException();
            }
        }
        #endregion

        #region GetTaskList()
        [Test]
        public void GetTaskList_WithoutTasks_ReturnsEmptyList()
        {
            // Arrange
            TestTaskManager taskManager = new(_counterMock.Object);

            // Act
            IReadOnlyDictionary<string, ProjectItem> actualTaskList = taskManager.GetTaskList();

            // Assert
            Assert.That(actualTaskList, Is.Empty);
        }

        [Test]
        public void GetTaskList_WithTasks_ReturnsFilledList()
        {
            // Arrange
            TestTaskManager taskManager = new(_counterMock.Object);

            taskManager.AddProject(ProjectName);

            // Act
            IReadOnlyDictionary<string, ProjectItem> actualTaskList = taskManager.GetTaskList();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(actualTaskList, Has.Count.EqualTo(1));
            });
        }
        #endregion

        #region AddProject()
        [Test]
        public void AddProject_Project_UniqueName_ReturnsSuccess()
        {
            // Arrange
            TestTaskManager taskManager = new(_counterMock.Object);

            // Act
            int taskListCountBefore = taskManager.GetTaskList().Count;

            CommandResponse response = taskManager.AddProject(ProjectName);

            int taskListCountAfter = taskManager.GetTaskList().Count;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(taskListCountBefore, Is.Zero);
                Assert.That(response.IsSuccess, Is.True);
                Assert.That(response.Content, Is.EqualTo($"Operation succeeded: The project with name \"{ProjectName}\" was created."));
                Assert.That(taskListCountAfter, Is.EqualTo(1));
            });
        }

        [Test]
        public void AddProject_Project_DuplicatedName_ReturnsFailure()
        {
            // Arrange
            TestTaskManager taskManager = new(_counterMock.Object);

            // Act
            int taskListCountBefore = taskManager.GetTaskList().Count;

            CommandResponse response = taskManager.AddProject(ProjectName);
            response = taskManager.AddProject(ProjectName);

            int taskListCountAfter = taskManager.GetTaskList().Count;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(taskListCountBefore, Is.Zero);
                Assert.That(response.IsFailure, Is.True);
                Assert.That(response.Content, Is.EqualTo($"Operation failed: Project with the same name already exists."));
                Assert.That(taskListCountAfter, Is.EqualTo(1));  // Not 2
            });
        }
        #endregion

        #region AddTask()
        [Test]
        public void AddTask_Project_Existing_ReturnsSuccess()
        {
            // Arrange
            _counterMock
                .Setup(counter => counter.GetNextTaskId())
                .Returns(default(long));

            TestTaskManager taskManager = new(_counterMock.Object);

            taskManager.AddProject(ProjectName);

            // Act
            IReadOnlyDictionary<string, ProjectItem> x = taskManager.GetTaskList();
            int tasksCountBefore = taskManager.GetTaskList()[ProjectName].Tasks.Count;

            CommandResponse response = taskManager.AddTask(ProjectName, TaskName);

            int tasksCountAfter = taskManager.GetTaskList()[ProjectName].Tasks.Count;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(tasksCountBefore, Is.EqualTo(0));
                Assert.That(response.IsSuccess, Is.True);
                Assert.That(response.Content, Is.EqualTo($"Operation succeeded: The task with name \"{TaskName}\" was added to the project \"{ProjectName}\"."));
                Assert.That(tasksCountAfter, Is.EqualTo(1));
                Assert.That(taskManager.GetTaskList()[ProjectName].Tasks.First().Description, Is.EqualTo(TaskName));
            });
        }

        [Test]
        public void AddTask_Project_NotExisting_ReturnsFailure()
        {
            // Arrange
            const string hobbyProject = "Hobby";

            _counterMock
                .Setup(counter => counter.GetNextTaskId())
                .Returns(default(long));

            TestTaskManager taskManager = new(_counterMock.Object);

            // Act
            CommandResponse response = taskManager.AddTask(hobbyProject, TaskName);  // Project was not added before

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.IsFailure, Is.True);
                Assert.That(response.Content, Is.EqualTo($"Operation failed: Could not find a project with the name \"{hobbyProject}\"."));
            });
        }

        [Test]
        public void AddTask_Counter_ThrowsException_ReturnsFailure()
        {
            // Arrange
            const string exceptionMessage = "Expected test exception.";

            _counterMock
                .Setup(counter => counter.GetNextTaskId())
                .Throws(new Exception(exceptionMessage));

            TestTaskManager taskManager = new(_counterMock.Object);

            taskManager.AddProject(ProjectName);

            // Act
            CommandResponse response = taskManager.AddTask(ProjectName, TaskName);

            // Assert
            Assert.Multiple(() =>
            {
                _counterMock.Verify(mock => mock.GetNextTaskId(), Times.Once);

                Assert.That(response.IsFailure, Is.True);
                Assert.That(response.Content, Does.StartWith($"Operation failed: {exceptionMessage}"));
            });
        }
        #endregion
    }
}

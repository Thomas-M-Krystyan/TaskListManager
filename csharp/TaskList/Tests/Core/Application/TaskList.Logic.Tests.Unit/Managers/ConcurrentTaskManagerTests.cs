using Moq;
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

        [SetUp]
        public void SetUp()
        {
            _counterMock.Reset();
            ConcurrentTaskManager.Reset();
        }

        #region GetTaskList()
        [Test]
        public void GetTaskList_WithoutTasks_ReturnsEmptyList()
        {
            // Arrange
            var taskManager = new TestTaskManager(_counterMock.Object);

            // Act
            var actualTaskList = taskManager.GetTaskList();

            // Assert
            Assert.That(actualTaskList, Is.Empty);
        }

        [Test]
        public void GetTaskList_WithTasks_ReturnsFilledList()
        {
            // Arrange
            _counterMock
                .Setup(counter => counter.GetNextProjectId())
                .Returns(default(long));

            var taskManager = new TestTaskManager(_counterMock.Object);

            taskManager.AddProject(ProjectName);

            // Act
            var actualTaskList = taskManager.GetTaskList();

            // Assert
            Assert.Multiple(() =>
            {
                _counterMock.Verify(mock => mock.GetNextProjectId(), Times.Once);

                Assert.That(actualTaskList, Has.Count.EqualTo(1));
            });
        }
        #endregion

        #region AddProject()
        [Test]
        public void AddProject_Project_UniqueName_ReturnsSuccess()
        {
            // Arrange
            _counterMock
                .Setup(counter => counter.GetNextProjectId())
                .Returns(default(long));

            var taskManager = new TestTaskManager(_counterMock.Object);

            // Act
            var taskListCountBefore = taskManager.GetTaskList().Count;

            var response = taskManager.AddProject(ProjectName);

            var taskListCountAfter = taskManager.GetTaskList().Count;

            // Assert
            Assert.Multiple(() =>
            {
                _counterMock.Verify(mock => mock.GetNextProjectId(), Times.Once);

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
            _counterMock
                .Setup(counter => counter.GetNextProjectId())
                .Returns(default(long));

            var taskManager = new TestTaskManager(_counterMock.Object);

            // Act
            var taskListCountBefore = taskManager.GetTaskList().Count;

            var response = taskManager.AddProject(ProjectName);
            response = taskManager.AddProject(ProjectName);

            var taskListCountAfter = taskManager.GetTaskList().Count;

            // Assert
            Assert.Multiple(() =>
            {
                _counterMock.Verify(mock => mock.GetNextProjectId(), Times.Exactly(2));

                Assert.That(taskListCountBefore, Is.Zero);
                Assert.That(response.IsFailure, Is.True);
                Assert.That(response.Content, Is.EqualTo($"Operation failed: Project with the same name already exists."));
                Assert.That(taskListCountAfter, Is.EqualTo(1));  // Not 2
            });
        }

        [Test]
        public void AddProject_Counter_ThrowsException_ReturnsFailure()
        {
            // Arrange
            const string exceptionMessage = "Expected test exception.";

            _counterMock
                .Setup(counter => counter.GetNextProjectId())
                .Throws(new Exception(exceptionMessage));

            var taskManager = new TestTaskManager(_counterMock.Object);

            // Act
            var response = taskManager.AddProject(ProjectName);
            var taskListCountAfter = taskManager.GetTaskList().Count;

            // Assert
            Assert.Multiple(() =>
            {
                _counterMock.Verify(mock => mock.GetNextProjectId(), Times.Once);

                Assert.That(response.IsFailure, Is.True);
                Assert.That(response.Content, Does.StartWith($"Operation failed: {exceptionMessage}"));
                Assert.That(taskListCountAfter, Is.Zero);
            });
        }
        #endregion

        #region AddTask()
        [Test]
        public void AddTask_Project_Existing_ReturnsSuccess()
        {
            // Arrange
            _counterMock
                .Setup(counter => counter.GetNextProjectId())
                .Returns(default(long));

            _counterMock
                .Setup(counter => counter.GetNextTaskId())
                .Returns(default(long));

            var taskManager = new TestTaskManager(_counterMock.Object);

            taskManager.AddProject(ProjectName);

            // Act
            var x = taskManager.GetTaskList();
            var tasksCountBefore = taskManager.GetTaskList()[ProjectName].Tasks.Count;

            var response = taskManager.AddTask(ProjectName, TaskName);

            var tasksCountAfter = taskManager.GetTaskList()[ProjectName].Tasks.Count;

            // Assert
            Assert.Multiple(() =>
            {
                _counterMock.Verify(mock => mock.GetNextProjectId(), Times.Once);
                _counterMock.Verify(mock => mock.GetNextTaskId(), Times.Once);

                Assert.That(tasksCountBefore, Is.EqualTo(0));
                Assert.That(response.IsSuccess, Is.True);
                Assert.That(response.Content, Is.EqualTo($"Operation succeeded: The task with name \"{TaskName}\" was added to the project \"{ProjectName}\"."));
                Assert.That(tasksCountAfter, Is.EqualTo(1));
                Assert.That(taskManager.GetTaskList()[ProjectName].Tasks.First().Value.Description, Is.EqualTo(TaskName));
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

            var taskManager = new TestTaskManager(_counterMock.Object);

            // Act
            var response = taskManager.AddTask(hobbyProject, TaskName);  // Project was not added before

            // Assert
            Assert.Multiple(() =>
            {
                _counterMock.Verify(mock => mock.GetNextTaskId(), Times.Never);

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
                .Setup(counter => counter.GetNextProjectId())
                .Returns(default(long));

            _counterMock
                .Setup(counter => counter.GetNextTaskId())
                .Throws(new Exception(exceptionMessage));

            var taskManager = new TestTaskManager(_counterMock.Object);

            taskManager.AddProject(ProjectName);

            // Act
            var response = taskManager.AddTask(ProjectName, TaskName);

            // Assert
            Assert.Multiple(() =>
            {
                _counterMock.Verify(mock => mock.GetNextProjectId(), Times.Once);
                _counterMock.Verify(mock => mock.GetNextTaskId(), Times.Once);

                Assert.That(response.IsFailure, Is.True);
                Assert.That(response.Content, Does.StartWith($"Operation failed: {exceptionMessage}"));
            });
        }
        #endregion

        #region CheckTask()
        [Test]
        public void CheckTask_MarkAsDone_TaskList_Empty_ReturnsFailure()
        {
            // Arrange
            const long taskId = 1;

            var taskManager = new TestTaskManager(_counterMock.Object);

            // Act
            var response = taskManager.CheckTask(taskId, true);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.IsFailure, Is.True);
                Assert.That(response.Content, Is.EqualTo($"Operation failed: Could not find a task with an ID of {taskId}."));
            });
        }

        [Test]
        public void CheckTask_MarkAsDone_Task_Unfinished_ReturnsSuccess()
        {
            // Arrange
            _counterMock
                .Setup(counter => counter.GetNextProjectId())
                .Returns(default(long));

            _counterMock
                .Setup(counter => counter.GetNextTaskId())
                .Returns(default(long));

            var taskManager = new TestTaskManager(_counterMock.Object);

            taskManager.AddProject(ProjectName);
            taskManager.AddTask(ProjectName, TaskName);

            var taskId = taskManager.GetTaskList()[ProjectName].Tasks.First().Value.Id;

            // Act
            var response = taskManager.CheckTask(taskId, true);

            // Assert
            Assert.Multiple(() =>
            {
                _counterMock.Verify(mock => mock.GetNextProjectId(), Times.Once);
                _counterMock.Verify(mock => mock.GetNextTaskId(), Times.Once);

                Assert.That(response.IsSuccess, Is.True);
                Assert.That(response.Content, Is.EqualTo($"Operation succeeded: The task with ID {taskId} was marked as finished."));
                Assert.That(taskManager.GetTaskList()[ProjectName].Tasks.First().Value.IsDone, Is.True);
            });
        }

        [Test]
        public void CheckTask_MarkAsUndone_Task_Finished_ReturnsSuccess()
        {
            // Arrange
            _counterMock
                .Setup(counter => counter.GetNextProjectId())
                .Returns(default(long));

            _counterMock
                .Setup(counter => counter.GetNextTaskId())
                .Returns(default(long));

            var taskManager = new TestTaskManager(_counterMock.Object);

            taskManager.AddProject(ProjectName);
            taskManager.AddTask(ProjectName, TaskName);

            var taskId = taskManager.GetTaskList()[ProjectName].Tasks.First().Value.Id;

            taskManager.CheckTask(taskId, true);

            // Act
            var response = taskManager.CheckTask(taskId, false);

            // Assert
            Assert.Multiple(() =>
            {
                _counterMock.Verify(mock => mock.GetNextProjectId(), Times.Once);
                _counterMock.Verify(mock => mock.GetNextTaskId(), Times.Once);

                Assert.That(response.IsSuccess, Is.True);
                Assert.That(response.Content, Is.EqualTo($"Operation succeeded: The task with ID {taskId} was marked as unfinished."));
                Assert.That(taskManager.GetTaskList()[ProjectName].Tasks.First().Value.IsDone, Is.False);
            });
        }
        #endregion
    }
}

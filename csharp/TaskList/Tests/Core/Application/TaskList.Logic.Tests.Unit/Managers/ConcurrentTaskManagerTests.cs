using Moq;
using System.Text.Json;
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
        private const long TaskId = 1;
        private const string ProjectName = "Work";
        private const string TaskName = "Task";

        private sealed class TestTaskManager(ICounterRegister counter) : ConcurrentTaskManager(counter)
        {
            public override CommandResponse DisplayAllTasks() => throw new NotImplementedException();

            public override CommandResponse DisplayTodayTasks() => throw new NotImplementedException();

            public override CommandResponse DisplayTasksByDeadline() => throw new NotImplementedException();
        }
        #endregion

        [SetUp]
        public void SetUp()
        {
            _counterMock.Reset();
        }

        #region GetAllProjects()
        [Test]
        public void GetAllProjects_WithoutTasks_ReturnsEmptyList()
        {
            // Arrange
            TestTaskManager taskManager = new(_counterMock.Object);

            // Act
            IReadOnlyDictionary<string, ProjectItem> actualCollection = taskManager.GetAllProjects();

            // Assert
            Assert.That(actualCollection, Is.Empty);
        }

        [Test]
        public void GetAllProjects_WithTasks_ReturnsFilledList()
        {
            // Arrange
            _ = _counterMock
                .Setup(counter => counter.GetNextProjectId())
                .Returns(default(long));

            TestTaskManager taskManager = new(_counterMock.Object);

            _ = taskManager.AddProject(ProjectName);

            // Act
            IReadOnlyDictionary<string, ProjectItem> actualCollection = taskManager.GetAllProjects();

            // Assert
            Assert.Multiple(() =>
            {
                _counterMock.Verify(mock => mock.GetNextProjectId(), Times.Once);

                Assert.That(actualCollection, Has.Count.EqualTo(1));
            });
        }

        [Test]
        public void GetAllProjects_IsTrulyImmutable()
        {
            // Arrange
            _ = _counterMock
                .Setup(counter => counter.GetNextProjectId())
                .Returns(default(long));

            _ = _counterMock
                .Setup(counter => counter.GetNextTaskId())
                .Returns(default(long));

            TestTaskManager taskManager = new(_counterMock.Object);

            _ = taskManager.AddProject(ProjectName);
            _ = taskManager.AddTask(ProjectName, TaskName);

            IReadOnlyDictionary<string, ProjectItem> taskList = taskManager.GetAllProjects();

            string initialOriginalSerializedTaskList = JsonSerializer.Serialize(taskManager.GetAllProjects());
            const string expectedOriginalSerializedTaskList =
                "{\"Work\":{\"Name\":\"Work\",\"TaskIds\":[0],\"OrderNumber\":0}}";

            // Assert before
            Assert.Multiple(() =>
            {
                _counterMock.Verify(mock => mock.GetNextProjectId(), Times.Once);
                _counterMock.Verify(mock => mock.GetNextTaskId(), Times.Once);

                Assert.That(initialOriginalSerializedTaskList, Is.EqualTo(expectedOriginalSerializedTaskList));
            });

            // Act (try to add new task ID inside of a project)
            taskList.First().Value.TaskIds.Add(999);

            // Assert after
            Assert.Multiple(() =>
            {
                string serializedTaskListAfterFirstModification = JsonSerializer.Serialize(taskManager.GetAllProjects());

                Assert.That(serializedTaskListAfterFirstModification, Is.EqualTo(expectedOriginalSerializedTaskList));
            });
        }
        #endregion

        #region GetAllTasks()
        [Test]
        public void GetAllTasks_WithoutTasks_ReturnsEmptyList()
        {
            // Arrange
            TestTaskManager taskManager = new(_counterMock.Object);

            // Act
            IReadOnlyDictionary<long, TaskItem> actualCollection = taskManager.GetAllTasks();

            // Assert
            Assert.That(actualCollection, Is.Empty);
        }

        [Test]
        public void GetAllTasks_WithTasks_ReturnsFilledList()
        {
            // Arrange
            _ = _counterMock
                .Setup(counter => counter.GetNextProjectId())
                .Returns(default(long));

            _ = _counterMock
                .Setup(counter => counter.GetNextTaskId())
                .Returns(default(long));

            TestTaskManager taskManager = new(_counterMock.Object);

            _ = taskManager.AddProject(ProjectName);
            _ = taskManager.AddTask(ProjectName, TaskName);

            // Act
            IReadOnlyDictionary<long, TaskItem> actualCollection = taskManager.GetAllTasks();

            // Assert
            Assert.Multiple(() =>
            {
                _counterMock.Verify(mock => mock.GetNextProjectId(), Times.Once);
                _counterMock.Verify(mock => mock.GetNextTaskId(), Times.Once);

                Assert.That(actualCollection, Has.Count.EqualTo(1));
            });
        }
        #endregion

        #region AddProject()
        [Test]
        public void AddProject_Project_UniqueName_ReturnsSuccess()
        {
            // Arrange
            _ = _counterMock
                .Setup(counter => counter.GetNextProjectId())
                .Returns(default(long));

            TestTaskManager taskManager = new(_counterMock.Object);

            // Act
            int taskListCountBefore = taskManager.GetAllProjects().Count;

            CommandResponse response = taskManager.AddProject(ProjectName);

            int taskListCountAfter = taskManager.GetAllProjects().Count;

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
            _ = _counterMock
                .Setup(counter => counter.GetNextProjectId())
                .Returns(default(long));

            TestTaskManager taskManager = new(_counterMock.Object);

            // Act
            int taskListCountBefore = taskManager.GetAllProjects().Count;

            CommandResponse response = taskManager.AddProject(ProjectName);
            response = taskManager.AddProject(ProjectName);

            int taskListCountAfter = taskManager.GetAllProjects().Count;

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

            _ = _counterMock
                .Setup(counter => counter.GetNextProjectId())
                .Throws(new Exception(exceptionMessage));

            TestTaskManager taskManager = new(_counterMock.Object);

            // Act
            CommandResponse response = taskManager.AddProject(ProjectName);
            int taskListCountAfter = taskManager.GetAllProjects().Count;

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
            _ = _counterMock
                .Setup(counter => counter.GetNextProjectId())
                .Returns(default(long));

            _ = _counterMock
                .Setup(counter => counter.GetNextTaskId())
                .Returns(default(long));

            TestTaskManager taskManager = new(_counterMock.Object);

            _ = taskManager.AddProject(ProjectName);

            // Act
            int tasksCountBefore = taskManager.GetAllProjects()[ProjectName].TaskIds.Count;

            CommandResponse response = taskManager.AddTask(ProjectName, TaskName);

            int tasksCountAfter = taskManager.GetAllProjects()[ProjectName].TaskIds.Count;

            // Assert
            Assert.Multiple(() =>
            {
                _counterMock.Verify(mock => mock.GetNextProjectId(), Times.Once);
                _counterMock.Verify(mock => mock.GetNextTaskId(), Times.Once);

                Assert.That(tasksCountBefore, Is.EqualTo(0));
                Assert.That(response.IsSuccess, Is.True);
                Assert.That(response.Content, Is.EqualTo($"Operation succeeded: The task with name \"{TaskName}\" was added to the project \"{ProjectName}\"."));
                Assert.That(tasksCountAfter, Is.EqualTo(1));

                long newTaskId = taskManager.GetAllProjects()[ProjectName].TaskIds[0];
                Assert.That(taskManager.GetAllTasks()[newTaskId].Name, Is.EqualTo(TaskName));
            });
        }

        [Test]
        public void AddTask_Project_NotExisting_ReturnsFailure()
        {
            // Arrange
            const string hobbyProject = "Hobby";

            _ = _counterMock
                .Setup(counter => counter.GetNextTaskId())
                .Returns(default(long));

            TestTaskManager taskManager = new(_counterMock.Object);

            // Act
            CommandResponse response = taskManager.AddTask(hobbyProject, TaskName);  // Project was not added before

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

            _ = _counterMock
                .Setup(counter => counter.GetNextProjectId())
                .Returns(default(long));

            _ = _counterMock
                .Setup(counter => counter.GetNextTaskId())
                .Throws(new Exception(exceptionMessage));

            TestTaskManager taskManager = new(_counterMock.Object);

            _ = taskManager.AddProject(ProjectName);

            // Act
            CommandResponse response = taskManager.AddTask(ProjectName, TaskName);

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
        public void CheckTask_MarkAsDone_Project_NotExisting_ReturnsFailure()
        {
            // Arrange
            TestTaskManager taskManager = new(_counterMock.Object);

            // Act
            CommandResponse response = taskManager.CheckTask(TaskId, true);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.IsFailure, Is.True);
                Assert.That(response.Content, Is.EqualTo($"Operation failed: Could not find a task with an ID of {TaskId}."));
            });
        }

        [Test]
        public void CheckTask_MarkAsDone_Task_NotExisting_ReturnsFailure()
        {
            // Arrange
            _ = _counterMock
                .Setup(counter => counter.GetNextProjectId())
                .Returns(default(long));

            TestTaskManager taskManager = new(_counterMock.Object);

            _ = taskManager.AddProject(ProjectName);

            // Act
            CommandResponse response = taskManager.CheckTask(TaskId, true);

            // Assert
            Assert.Multiple(() =>
            {
                _counterMock.Verify(mock => mock.GetNextProjectId(), Times.Once);

                Assert.That(response.IsFailure, Is.True);
                Assert.That(response.Content, Is.EqualTo($"Operation failed: Could not find a task with an ID of {TaskId}."));
            });
        }

        [Test]
        public void CheckTask_MarkAsDone_Task_Existing_Unfinished_ReturnsSuccess()
        {
            // Arrange
            _ = _counterMock
                .Setup(counter => counter.GetNextProjectId())
                .Returns(default(long));

            _ = _counterMock
                .Setup(counter => counter.GetNextTaskId())
                .Returns(default(long));

            TestTaskManager taskManager = new(_counterMock.Object);

            _ = taskManager.AddProject(ProjectName);
            _ = taskManager.AddTask(ProjectName, TaskName);

            long taskId = taskManager.GetAllProjects()[ProjectName].TaskIds[0];

            // Act
            CommandResponse response = taskManager.CheckTask(taskId, true);

            // Assert
            Assert.Multiple(() =>
            {
                _counterMock.Verify(mock => mock.GetNextProjectId(), Times.Once);
                _counterMock.Verify(mock => mock.GetNextTaskId(), Times.Once);

                Assert.That(response.IsSuccess, Is.True);
                Assert.That(response.Content, Is.EqualTo($"Operation succeeded: The task with ID {taskId} was marked as finished."));
                Assert.That(taskManager.GetAllTasks()[taskId].IsDone, Is.True);
            });
        }

        [Test]
        public void CheckTask_MarkAsUndone_Task_Existing_Finished_ReturnsSuccess()
        {
            // Arrange
            _ = _counterMock
                .Setup(counter => counter.GetNextProjectId())
                .Returns(default(long));

            _ = _counterMock
                .Setup(counter => counter.GetNextTaskId())
                .Returns(default(long));

            TestTaskManager taskManager = new(_counterMock.Object);

            _ = taskManager.AddProject(ProjectName);
            _ = taskManager.AddTask(ProjectName, TaskName);

            long taskId = taskManager.GetAllProjects()[ProjectName].TaskIds[0];

            _ = taskManager.CheckTask(taskId, true);

            // Act
            CommandResponse response = taskManager.CheckTask(taskId, false);

            // Assert
            Assert.Multiple(() =>
            {
                _counterMock.Verify(mock => mock.GetNextProjectId(), Times.Once);
                _counterMock.Verify(mock => mock.GetNextTaskId(), Times.Once);

                Assert.That(response.IsSuccess, Is.True);
                Assert.That(response.Content, Is.EqualTo($"Operation succeeded: The task with ID {taskId} was marked as unfinished."));
                Assert.That(taskManager.GetAllTasks()[taskId].IsDone, Is.False);
            });
        }
        #endregion

        #region SetDeadline()
        [Test]
        public void SetDeadline_Project_NotExisting_ReturnsFailure()
        {
            // Arrange
            TestTaskManager taskManager = new(_counterMock.Object);

            // Act
            CommandResponse response = taskManager.SetDeadline(TaskId, DateOnly.MinValue);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.IsFailure, Is.True);
                Assert.That(response.Content, Is.EqualTo($"Operation failed: Could not find a task with an ID of {TaskId}."));
            });
        }

        [Test]
        public void SetDeadline_Task_NotExisting_ReturnsFailure()
        {
            // Arrange
            _ = _counterMock
                .Setup(counter => counter.GetNextProjectId())
                .Returns(default(long));

            TestTaskManager taskManager = new(_counterMock.Object);

            _ = taskManager.AddProject(ProjectName);

            // Act
            CommandResponse response = taskManager.SetDeadline(TaskId, DateOnly.MinValue);

            // Assert
            Assert.Multiple(() =>
            {
                _counterMock.Verify(mock => mock.GetNextProjectId(), Times.Once);

                Assert.That(response.IsFailure, Is.True);
                Assert.That(response.Content, Is.EqualTo($"Operation failed: Could not find a task with an ID of {TaskId}."));
            });
        }

        [Test]
        public void SetDeadline_Task_Existing_ReturnsSuccess()
        {
            // Arrange
            _ = _counterMock
                .Setup(counter => counter.GetNextProjectId())
                .Returns(default(long));

            _ = _counterMock
                .Setup(counter => counter.GetNextTaskId())
                .Returns(default(long));

            TestTaskManager taskManager = new(_counterMock.Object);

            _ = taskManager.AddProject(ProjectName);
            _ = taskManager.AddTask(ProjectName, TaskName);

            long taskId = taskManager.GetAllProjects()[ProjectName].TaskIds[0];

            // Act
            CommandResponse response = taskManager.SetDeadline(taskId, DateOnly.MinValue);

            // Assert
            Assert.Multiple(() =>
            {
                _counterMock.Verify(mock => mock.GetNextProjectId(), Times.Once);
                _counterMock.Verify(mock => mock.GetNextTaskId(), Times.Once);

                Assert.That(response.IsSuccess, Is.True);
                Assert.That(response.Content, Is.EqualTo($"Operation succeeded: The deadline for the task with ID {taskId} was set to 01.01.0001."));
                Assert.That(taskManager.GetAllTasks()[taskId].Deadline, Is.Not.EqualTo(DateOnly.MaxValue));
            });
        }
        #endregion
    }
}

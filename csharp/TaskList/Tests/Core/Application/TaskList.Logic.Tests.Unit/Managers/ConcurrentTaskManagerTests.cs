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
        }

        #region GetTaskList()
        [Test]
        public void GetTaskList_WithoutTasks_ReturnsEmptyList()
        {
            // Arrange
            TestTaskManager taskManager = new(_counterMock.Object);

            // Act
            IReadOnlyDictionary<string, Domain.Models.ProjectItem> actualTaskList = taskManager.GetTaskList();

            // Assert
            Assert.That(actualTaskList, Is.Empty);
        }

        [Test]
        public void GetTaskList_WithTasks_ReturnsFilledList()
        {
            // Arrange
            _ = _counterMock
                .Setup(counter => counter.GetNextProjectId())
                .Returns(default(long));

            TestTaskManager taskManager = new(_counterMock.Object);

            _ = taskManager.AddProject(ProjectName);

            // Act
            IReadOnlyDictionary<string, Domain.Models.ProjectItem> actualTaskList = taskManager.GetTaskList();

            // Assert
            Assert.Multiple(() =>
            {
                _counterMock.Verify(mock => mock.GetNextProjectId(), Times.Once);

                Assert.That(actualTaskList, Has.Count.EqualTo(1));
            });
        }

        [Test]
        public void GetTaskList_IsTrulyImmutable()
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

            IReadOnlyDictionary<string, ProjectItem> taskList = taskManager.GetTaskList();

            string initialOriginalSerializedTaskList = JsonSerializer.Serialize(taskManager.GetTaskList());
            const string expectedOriginalSerializedTaskList =
                "{\"Work\":{\"Id\":0,\"Name\":\"Work\",\"Tasks\":{\"0\":{\"Id\":0,\"Description\":\"Task\",\"IsDone\":false}}}}";

            // Assert before
            Assert.Multiple(() =>
            {
                _counterMock.Verify(mock => mock.GetNextProjectId(), Times.Once);
                _counterMock.Verify(mock => mock.GetNextTaskId(), Times.Once);

                Assert.That(initialOriginalSerializedTaskList, Is.EqualTo(expectedOriginalSerializedTaskList));
            });

            // Act #1 (try to add new task inside of a project)
            taskList.First().Value.Tasks.Add(2, new TaskItem(2, "New task"));

            // Assert after #1
            Assert.Multiple(() =>
            {
                string serializedTaskListAfterFirstModification = JsonSerializer.Serialize(taskManager.GetTaskList());

                Assert.That(serializedTaskListAfterFirstModification, Is.EqualTo(expectedOriginalSerializedTaskList));
            });

            // Act #2 (try to modify existing task)
            TaskItem firstTask = taskList.First().Value.Tasks.First().Value;
            firstTask.IsDone = true;

            // Assert after #2
            Assert.Multiple(() =>
            {
                string serializedTaskListAfterSecondModification = JsonSerializer.Serialize(taskManager.GetTaskList());

                Assert.That(serializedTaskListAfterSecondModification, Is.EqualTo(expectedOriginalSerializedTaskList));
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
            int taskListCountBefore = taskManager.GetTaskList().Count;

            CommandResponse response = taskManager.AddProject(ProjectName);

            int taskListCountAfter = taskManager.GetTaskList().Count;

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
            int taskListCountBefore = taskManager.GetTaskList().Count;

            CommandResponse response = taskManager.AddProject(ProjectName);
            response = taskManager.AddProject(ProjectName);

            int taskListCountAfter = taskManager.GetTaskList().Count;

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
            int taskListCountAfter = taskManager.GetTaskList().Count;

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
            IReadOnlyDictionary<string, Domain.Models.ProjectItem> x = taskManager.GetTaskList();
            int tasksCountBefore = taskManager.GetTaskList()[ProjectName].Tasks.Count;

            CommandResponse response = taskManager.AddTask(ProjectName, TaskName);

            int tasksCountAfter = taskManager.GetTaskList()[ProjectName].Tasks.Count;

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
        public void CheckTask_MarkAsDone_TaskList_Empty_ReturnsFailure()
        {
            // Arrange
            const long taskId = 1;

            TestTaskManager taskManager = new(_counterMock.Object);

            // Act
            CommandResponse response = taskManager.CheckTask(taskId, true);

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
            _ = _counterMock
                .Setup(counter => counter.GetNextProjectId())
                .Returns(default(long));

            _ = _counterMock
                .Setup(counter => counter.GetNextTaskId())
                .Returns(default(long));

            TestTaskManager taskManager = new(_counterMock.Object);

            _ = taskManager.AddProject(ProjectName);
            _ = taskManager.AddTask(ProjectName, TaskName);

            long taskId = taskManager.GetTaskList()[ProjectName].Tasks.First().Value.Id;

            // Act
            CommandResponse response = taskManager.CheckTask(taskId, true);

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
            _ = _counterMock
                .Setup(counter => counter.GetNextProjectId())
                .Returns(default(long));

            _ = _counterMock
                .Setup(counter => counter.GetNextTaskId())
                .Returns(default(long));

            TestTaskManager taskManager = new(_counterMock.Object);

            _ = taskManager.AddProject(ProjectName);
            _ = taskManager.AddTask(ProjectName, TaskName);

            long taskId = taskManager.GetTaskList()[ProjectName].Tasks.First().Value.Id;

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
                Assert.That(taskManager.GetTaskList()[ProjectName].Tasks.First().Value.IsDone, Is.False);
            });
        }
        #endregion
    }
}

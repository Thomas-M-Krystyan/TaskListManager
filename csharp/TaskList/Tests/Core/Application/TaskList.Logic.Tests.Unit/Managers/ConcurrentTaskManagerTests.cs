using TaskList.Logic.Managers;
using TaskList.Logic.Responses;

namespace TaskList.Logic.Tests.Unit.Managers
{
    [TestFixture]
    public sealed class ConcurrentTaskManagerTests
    {
        #region Data
        private const string ProjectName = "Work";

        private sealed class TestTaskManager() : ConcurrentTaskManager()
        {
        }
        #endregion

        #region AddProject()
        [Test]
        public void AddProject_Project_UniqueName_ReturnsSuccess()
        {
            // Arrange
            TestTaskManager taskManager = new();

            // Act
            int taskListCountBefore = taskManager.TaskList.Count;

            CommandResponse response = taskManager.AddProject(ProjectName);

            int taskListCountAfter = taskManager.TaskList.Count;

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
            TestTaskManager taskManager = new();

            // Act
            int taskListCountBefore = taskManager.TaskList.Count;

            CommandResponse response = taskManager.AddProject(ProjectName);
            response = taskManager.AddProject(ProjectName);

            int taskListCountAfter = taskManager.TaskList.Count;

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
    }
}

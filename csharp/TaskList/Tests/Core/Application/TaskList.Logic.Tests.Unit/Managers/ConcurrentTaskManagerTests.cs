using TaskList.Logic.Managers;

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
            var taskManager = new TestTaskManager();

            // Act
            var taskListCountBefore = taskManager.TaskList.Count;

            var response = taskManager.AddProject(ProjectName);

            var taskListCountAfter = taskManager.TaskList.Count;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(taskListCountBefore, Is.Zero);
                Assert.That(response, Is.True);
                Assert.That(taskListCountAfter, Is.EqualTo(1));
            });
        }
        #endregion
    }
}

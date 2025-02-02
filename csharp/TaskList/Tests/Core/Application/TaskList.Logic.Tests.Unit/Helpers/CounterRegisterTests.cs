using TaskList.Logic.Helpers;

namespace TaskList.Logic.Tests.Unit.Helpers
{
    [TestFixture]
    public sealed class CounterRegisterTests
    {
        #region Project IDs
        [Test]
        public void GetNextProjectId_Called_OneTime_ReturnsIncrementedId()
        {
            // Act
            long result = new CounterRegister().GetNextProjectId();

            // Assert
            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public void GetNextProjectId_Called_ManyTimes_ReturnsIncrementedId()
        {
            // Act
            CounterRegister counterRegister = new();

            long result1 = counterRegister.GetNextProjectId();
            long result2 = counterRegister.GetNextProjectId();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result1, Is.EqualTo(1));
                Assert.That(result2, Is.EqualTo(2));
            });
        }
        #endregion

        #region Task IDs
        [Test]
        public void GetNextTaskId_Called_OneTime_ReturnsIncrementedId()
        {
            // Act
            long result = new CounterRegister().GetNextTaskId();

            // Assert
            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public void GetNextTaskId_Called_ManyTimes_ReturnsIncrementedId()
        {
            // Act
            CounterRegister counterRegister = new();

            long result1 = counterRegister.GetNextTaskId();
            long result2 = counterRegister.GetNextTaskId();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result1, Is.EqualTo(1));
                Assert.That(result2, Is.EqualTo(2));
            });
        }
        #endregion
    }
}

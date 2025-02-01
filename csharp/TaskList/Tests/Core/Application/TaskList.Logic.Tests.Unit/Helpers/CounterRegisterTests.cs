using TaskList.Logic.Helpers;

namespace TaskList.Logic.Tests.Unit.Helpers
{
    [TestFixture]
    public sealed class CounterRegisterTests
    {
        private readonly CounterRegister _counterRegister = new();

        [SetUp]
        public void TearDown()
        {
            CounterRegister.Reset();
        }

        #region Task IDs
        [Test]
        public void GetNextTaskId_Called_OneTime_ReturnsIncrementedId()
        {
            // Act
            long result = _counterRegister.GetNextTaskId();

            // Assert
            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public void GetNextTaskId_Called_ManyTimes_ReturnsIncrementedId()
        {
            // Act
            long result1 = _counterRegister.GetNextTaskId();
            long result2 = _counterRegister.GetNextTaskId();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result1, Is.EqualTo(1));
                Assert.That(result2, Is.EqualTo(2));
            });
        }

        [Test]
        public void GetNextTaskId_Created_MultipleTimes_ReturnsIncrementedId()
        {
            // Act
            long result1 = _counterRegister.GetNextTaskId();
            long result2 = new CounterRegister().GetNextTaskId();  // All instances should use the same shared counter
            long result3 = _counterRegister.GetNextTaskId();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result1, Is.EqualTo(1));
                Assert.That(result2, Is.EqualTo(2));
                Assert.That(result3, Is.EqualTo(3));
            });
        }
        #endregion
    }
}

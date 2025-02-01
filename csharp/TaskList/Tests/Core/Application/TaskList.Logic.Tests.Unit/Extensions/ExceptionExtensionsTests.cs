using TaskList.Logic.Extensions;

namespace TaskList.Logic.Tests.Unit.Extensions
{
    [TestFixture]
    public sealed class ExceptionExtensionsTests
    {
        #region GetMessage()
        [Test]
        public void GetMessage_Default_ExceptionMessage_ReturnsExpectedString()
        {
            // Act
            string message = new Exception().GetMessage();

            // Assert
            Assert.That(message, Is.EqualTo("Exception of type 'System.Exception' was thrown"));
        }

        [Test]
        public void GetMessage_Custom_Message_ReturnsExpectedString()
        {
            // Arrange
            const string exceptionMessage = "Custom exception message";

            // Act
            string message = new Exception(exceptionMessage).GetMessage();

            // Assert
            Assert.That(message, Is.EqualTo(exceptionMessage));
        }

        [Test]
        public void GetMessage_Custom_InnerMessage_ReturnsExpectedString()
        {
            // Arrange
            const string exceptionMessage = "Outer exception message";
            const string exceptionInnerMessage = "Inner exception message";

            // Act
            string message = new Exception(exceptionMessage,
                new Exception(exceptionInnerMessage)).GetMessage();

            // Assert
            Assert.That(message, Is.EqualTo(exceptionInnerMessage));
        }
        #endregion
    }
}
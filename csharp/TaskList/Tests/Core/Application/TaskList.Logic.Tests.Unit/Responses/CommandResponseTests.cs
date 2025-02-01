using TaskList.Logic.Responses;

namespace TaskList.Logic.Tests.Unit.Responses
{
    [TestFixture]
    public sealed class CommandResponseTests
    {
        #region Data
        private const string TestMessage = "Test message";
        private const string StandardConfirmation = "Operation succeeded";
        #endregion

        #region Success
        [TestCase("", $"{StandardConfirmation}.")]
        [TestCase(" ", $"{StandardConfirmation}.")]
        [TestCase(TestMessage, $"{StandardConfirmation}: {TestMessage}.")]
        public void Success_Content_ReturnsExpectedResult(string content, string expectedMessage)
        {
            // Act
            CommandResponse response = CommandResponse.Success(content);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.IsSuccess, Is.True);
                Assert.That(response.IsFailure, Is.False);
                Assert.That(response.Content, Is.EqualTo(expectedMessage));
            });
        }

        [Test]
        public void Success_Message_ReturnsExpectedResult()
        {
            // Act
            CommandResponse response = CommandResponse.Success(content: TestMessage);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.IsSuccess, Is.True);
                Assert.That(response.IsFailure, Is.False);
                Assert.That(response.Content, Is.EqualTo($"{StandardConfirmation}: {TestMessage}."));
            });
        }
        #endregion

        #region Failure
        [Test]
        public void Failure_String_ReturnsExpectedResult()
        {
            // Act
            CommandResponse response = CommandResponse.Failure(TestMessage);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.IsSuccess, Is.False);
                Assert.That(response.IsFailure, Is.True);
                Assert.That(response.Content, Is.EqualTo($"Operation failed: {TestMessage}."));
            });
        }

        [Test]
        public void Failure_Exception_ReturnsExpectedResult()
        {
            // Arrange
            Exception exception = new(TestMessage);

            // Act
            CommandResponse response = CommandResponse.Failure(exception);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.IsSuccess, Is.False);
                Assert.That(response.IsFailure, Is.True);
                Assert.That(response.Content, Is.EqualTo($"Operation failed: {TestMessage}."));
            });
        }
        #endregion
    }
}

using Chat.Application.Models;
using FluentAssertions;

namespace Chat.Application.UnitTests.Models;
public class MessageTests
{
    [Fact]
    public void Given_Message_The_Formatted_Text_Is_As_Expected()
    {
        // Given
        var sut =
            new Message
            {
                CreatedOn = new DateTime(2022,12,21,10,11,22,33,DateTimeKind.Utc),
                CreatedBy = "pepe",
                Text = "Hi! I'm pepe, how are you?"
            };

        var expectedText = "[2022-12-21 10:11:22] pepe says: Hi! I'm pepe, how are you?";

        // When
        var result = sut.FormattedText;

        // Then
        result.Should().Be(expectedText);

    }
}

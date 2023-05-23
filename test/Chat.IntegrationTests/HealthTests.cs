using System.Net;
using Chat.IntegrationTests.TestSupport;
using FluentAssertions;

namespace Chat.IntegrationTests;
public class HealthTests
    : IntegrationTest
{
    [Fact]
    public async Task Given_Health_Endpoint_When_Sending_Get_Request_It_Returns_200_Ok()
    {
        // Given
        var healthPath = "health";

        // When
        var result = await HttpClient.GetAsync(healthPath);

        // Then
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await result.Content.ReadAsStringAsync();
        payload.Should().Contain("Environment=Test");
        payload.Should().Contain("SecretWord=melón");
    }
}

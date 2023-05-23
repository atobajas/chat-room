using Chat.Application.Models;
using Chat.IntegrationTests.TestSupport;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;

namespace Chat.IntegrationTests;
public class MessagesTests
    : IntegrationTest
{
    [Fact]
    public async Task Given_Chat_And_Two_Users_And_Two_Messages_When_Getting_All_Messages_Then_All_Expected_Messages_Are_Retrieved()
    {
        // Given
        
        // Step 1: Create chat room
        var createChatResponse = await HttpClient.PostAsync("api/chats", null);
        if (!createChatResponse.IsSuccessStatusCode)
        {
            throw new Exception("Could not create chat. Test cannot continue");
        }
        var createChatResponseText = await createChatResponse.Content.ReadAsStringAsync(); // e.g: "0e45de22-04f3-40e5-b6c9-93c277494a91"
        // Tweak to remove quotes
        var chatId = createChatResponseText.Replace("\"", string.Empty); // e.g: 0e45de22-04f3-40e5-b6c9-93c277494a91

        // Step 2: Join Users
        var userOnePayload = new User("pepe", "conn1");
        var userTwoPayload = new User("lola", "conn2");

        var joinUserOneResponse = await HttpClient.PostAsJsonAsync($"api/chats/{chatId}/users", userOnePayload);
        var joinUserTwoResponse = await HttpClient.PostAsJsonAsync($"api/chats/{chatId}/users", userTwoPayload);
        if (!joinUserOneResponse.IsSuccessStatusCode || !joinUserTwoResponse.IsSuccessStatusCode)
        {
            throw new Exception("Could not join user. Test cannot continue");
        }

        // Step 3: Send Message
        var userOneMessage = "Hola a todos, yo soy Pepe";
        var userTwoMessage = "Hola Pepe, yo soy Lola";

        var sendMessageOneResponse =
            await HttpClient.PostAsJsonAsync(
                $"api/chats/{chatId}/users/{userOnePayload.Username}/messages",
                userOneMessage);
        var sendMessageTwoResponse =
            await HttpClient.PostAsJsonAsync(
                $"api/chats/{chatId}/users/{userTwoPayload.Username}/messages",
                userTwoMessage);
        if (!sendMessageOneResponse.IsSuccessStatusCode || !sendMessageTwoResponse.IsSuccessStatusCode)
        {
            throw new Exception("Could not send messages. Test cannot continue");
        }

        // When
        var result = await HttpClient.GetAsync($"api/chats/{chatId}/messages");

        // Then
        var json = await result.Content.ReadAsStringAsync();
        var serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        var messages = (JsonSerializer.Deserialize<IEnumerable<Message>>(json, serializerOptions)
                       ?? Array.Empty<Message>()).ToList();

        messages.Should().HaveCount(2);
    }
}

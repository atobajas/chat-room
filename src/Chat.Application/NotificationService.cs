using System.Net.Http.Json;

namespace Chat.Application;

public interface INotificationService
{
    Task SendNotification(string connectionId, string message);
}

public class NotificationService
    : INotificationService
{
    private readonly HttpClient _httpClient;

    public NotificationService(NotificationServiceConfiguration notificationServiceConfiguration)
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri(notificationServiceConfiguration.Host);
    }

    public async Task SendNotification(string connectionId, string message)
    {
        var jsonObject =
            new
            {
                Notification = message
            };
        var result = await _httpClient.PostAsJsonAsync($"@connections/{connectionId}", jsonObject);
        if (!result.IsSuccessStatusCode)
        {
            // Do something?
        }
    }
}

public class NotificationServiceConfiguration
{
    public string Host { get; init; } = "http://localhost:8080";

    public static NotificationServiceConfiguration Default => new();
}

namespace Chat.Application.Models
{
    public class User
    {
        public string Username { get; }
        public string ConnectionId { get; }
        public User(string username, string connectionId)
        {
            Username = username;
            ConnectionId = connectionId;
        }
    }
}

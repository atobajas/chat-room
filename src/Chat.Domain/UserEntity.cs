namespace Chat.Domain
{
    public class UserEntity
    {
        public string Username { get; set; } = string.Empty;
        public string ConnectionId { get; set; } = null!;
        public List<MessageEntity> Messages = null!;
        public ChatEntity Chat { get; set; } = null!;
        public Guid ChatId { get; set; }
    }
}

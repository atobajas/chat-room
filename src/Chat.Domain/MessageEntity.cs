namespace Chat.Domain
{
    public class MessageEntity
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public UserEntity User { get; set; } = null!;
        public string Username { get; set; } = string.Empty;
    }
}

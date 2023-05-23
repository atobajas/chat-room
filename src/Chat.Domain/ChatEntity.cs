namespace Chat.Domain
{
    public class ChatEntity
    {
        public Guid Id { get; set; }
        public List<UserEntity> Users = null!;
    }
}

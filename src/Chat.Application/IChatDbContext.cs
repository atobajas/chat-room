using Chat.Domain;
using Microsoft.EntityFrameworkCore;

namespace Chat.Application
{
    public interface IChatDbContext
    {
        DbSet<ChatEntity> Chats { get; }
        DbSet<MessageEntity> Messages { get; }
        DbSet<UserEntity> Users { get; }
        Task<int> SaveChangesAsync();
    }
}

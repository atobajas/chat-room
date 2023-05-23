using Chat.Application;
using Chat.Domain;
using Microsoft.EntityFrameworkCore;

namespace Chat.Persistence.SqlServer
{
    public class ChatDbContext : DbContext, IChatDbContext
    {
        public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options)
        {
        }

        public DbSet<ChatEntity> Chats { get; set; } = null!;
        public DbSet<MessageEntity> Messages { get; set; } = null!;
        public DbSet<UserEntity> Users { get; set; } = null!;

        public Task<int> SaveChangesAsync()
        {
            return base.SaveChangesAsync();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .Entity<UserEntity>()
                .HasKey(u => u.Username);

            modelBuilder
                .Entity<MessageEntity>()
                .HasOne(m => m.User)
                .WithMany(u => u.Messages)
                .HasForeignKey(m => m.Username);

            modelBuilder
                .Entity<UserEntity>()
                .HasOne(u => u.Chat)
                .WithMany(c => c.Users)
                .HasForeignKey(u => u.ChatId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

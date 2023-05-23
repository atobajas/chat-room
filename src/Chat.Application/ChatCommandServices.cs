using Chat.Application.Models;
using Chat.Domain;
using Microsoft.EntityFrameworkCore;

namespace Chat.Application
{
    public class ChatCommandServices
    {
        private readonly IChatDbContext _chatDbContext;
        private readonly INotificationService _notificationService;

        public ChatCommandServices(IChatDbContext chatDbContext, INotificationService notificationService)
        {
            _chatDbContext = chatDbContext;
            _notificationService = notificationService;
        }

        public async Task<Guid> Create()
        {
            var chatEntity = new ChatEntity();
            _chatDbContext.Chats.Add(chatEntity);

            await _chatDbContext.SaveChangesAsync();
            return chatEntity.Id;
        }

        public async Task Join(Guid chatId, User user)
        {
            var userEntity =
                new UserEntity
                {
                    ChatId = chatId,
                    Username = user.Username,
                    ConnectionId = user.ConnectionId
                };

            _chatDbContext.Users.Add(userEntity);
            await _chatDbContext.SaveChangesAsync();
        }

        public async Task SendMessage(Guid chatId, string username, string text)
        {
            var messageEntity =
                new MessageEntity
                {
                    CreatedOn = DateTime.Now,
                    Username = username,
                    Text = text
                };
            _chatDbContext.Messages.Add(messageEntity);

            await NotifyAllUsers(chatId, messageEntity);

            await _chatDbContext.SaveChangesAsync();
        }

        private async Task NotifyAllUsers(Guid chatId, MessageEntity messageEntity)
        {
            var message = new Message
            {
                Text = messageEntity.Text,
                CreatedBy = messageEntity.Username,
                CreatedOn = messageEntity.CreatedOn
            };

            var allConnectionIds =
                await _chatDbContext
                .Users
                .Where(u => u.ChatId == chatId)
                .Select(u => u.ConnectionId)
                .ToListAsync();

            var notificationsTasks = new List<Task>();

            foreach (var connectionId in allConnectionIds)
            {
                var task = _notificationService.SendNotification(connectionId, message.FormattedText);
                notificationsTasks.Add(task);
            }

            await Task.WhenAll(notificationsTasks);
        }
    }
}

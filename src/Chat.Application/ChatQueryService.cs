using Chat.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace Chat.Application
{
    public class ChatQueryService
    {
        private readonly IChatDbContext _chatDbContext;

        public ChatQueryService(IChatDbContext chatDbContext)
        {
            _chatDbContext = chatDbContext;
        }
        public async Task<IEnumerable<Message>> GetAllMessages(Guid chatId)
        {
            var userEntities =
                await _chatDbContext
                .Users
                .Where(u => u.ChatId == chatId)
                .SelectMany(u => u.Messages)
                .OrderBy(m => m.CreatedOn)
                .ToListAsync();

            var messages =
                userEntities
                .Select(m => new Message
                {
                    CreatedBy = m.Username,
                    CreatedOn = m.CreatedOn,
                    Text = m.Text
                });

            return messages;
        }
    }
}

using Chat.Application;
using Chat.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Chat.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatsController : ControllerBase
    {
        private ChatCommandServices _chatCommandServices;
        private ChatQueryService _chatQueryServices;

        public ChatsController(ChatCommandServices chatCommandServices, ChatQueryService chatQueryService)
        {
            _chatCommandServices = chatCommandServices;
            _chatQueryServices = chatQueryService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateChat()
        {
            var id = await _chatCommandServices.Create();
            return Ok(id);
        }

        [HttpPost("{id:guid}/users")]
        public async Task<IActionResult> Join(Guid id, [FromBody] User user)
        {
            await _chatCommandServices.Join(id, user);
            return Ok();
        }

        [HttpPost("{id:guid}/users/{username}/messages")]
        public async Task<IActionResult> SendMessage(Guid id, string username, [FromBody] string text)
        {
            await _chatCommandServices.SendMessage(id, username, text);
            return Ok();
        }

        [HttpGet("{id:guid}/messages")]
        public async Task<IEnumerable<Message>> GetMessages(Guid id)
        {
            var messages = await _chatQueryServices.GetAllMessages(id);
            return messages;
        }

    }
}

using Arak.BLL.DTO;
using Arak.BLL.Service.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Arak.PLL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ConversationsController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public ConversationsController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        private string GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        /// <summary>
        /// GET api/Conversations — List all conversations for the current user.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetConversations()
        {
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();

            var conversations = await _messageService.GetUserConversationsAsync(currentUserId);
            return Ok(conversations);
        }

        /// <summary>
        /// GET api/Conversations/{userId}/messages?page=1&amp;pageSize=50 — Get paginated chat history.
        /// </summary>
        [HttpGet("{userId}/messages")]
        public async Task<IActionResult> GetConversationMessages(
            string userId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();

            var messages = await _messageService.GetConversationHistoryAsync(currentUserId, userId, page, pageSize);
            return Ok(messages);
        }

        /// <summary>
        /// POST api/Conversations/{userId}/messages — Send a message to a user.
        /// </summary>
        [HttpPost("{userId}/messages")]
        public async Task<IActionResult> SendMessage(string userId, [FromBody] DtoSendMessage dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var senderId = GetCurrentUserId();
            if (string.IsNullOrEmpty(senderId))
                return Unauthorized();

            try
            {
                var message = await _messageService.SendMessageAsync(dto, senderId, userId);
                return CreatedAtAction(
                    nameof(GetConversationMessages),
                    new { userId },
                    message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// PATCH api/Conversations/{userId}/messages/{messageId}/read — Mark a single message as read.
        /// </summary>
        [HttpPatch("{userId}/messages/{messageId}/read")]
        public async Task<IActionResult> MarkAsRead(string userId, int messageId)
        {
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();

            var success = await _messageService.MarkAsReadAsync(messageId, currentUserId);
            if (!success)
                return NotFound("Message not found or you do not have permission to mark it as read.");

            return Ok(new { success = true });
        }

        /// <summary>
        /// PATCH api/Conversations/{userId}/read — Mark all messages in a conversation as read.
        /// </summary>
        [HttpPatch("{userId}/read")]
        public async Task<IActionResult> MarkConversationAsRead(string userId)
        {
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();

            var count = await _messageService.MarkConversationAsReadAsync(currentUserId, userId);
            return Ok(new { success = true, markedCount = count });
        }
    }
}

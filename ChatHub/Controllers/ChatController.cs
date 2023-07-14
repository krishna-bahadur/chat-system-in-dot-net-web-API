using ChatHub.BLL.DTOs;
using ChatHub.BLL.Services.Implementation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChatHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IHubContext<ChatServices> _chatHubContext;

        public ChatController(IHubContext<ChatServices> chatHubContext)
        {
            _chatHubContext = chatHubContext;
        }

        [HttpPost("sendMessage")]
        public async Task<IActionResult> SendMessage(string userId, string message)
        {
            // Validate userId, message, and perform any necessary checks

            // Send the message to the specified user
            //await _chatHubContext.Clients.User(userId).SendAsync("ReceiveMessage", message);           
            await _chatHubContext.Clients.All.SendAsync("ReceiveMessage", message);

            return Ok();
        }
    }
}

using ChatHub.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatHub.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageServices _messageServices;
        public MessageController(IMessageServices messageServices)
        {
            _messageServices = messageServices;
        }
        [HttpGet]
        [Route("GetMessageOfPrivateChat/{senderusername}/{receiverusername}")]
        public async Task<IActionResult> GetMessageOfPrivateChat(string senderusername, string receiverusername)
        {
            var result = await _messageServices.GetMessageOfPrivateChat(senderusername, receiverusername);
            if (!result.Success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return StatusCode(StatusCodes.Status200OK, result.Data);

        }
    }
}

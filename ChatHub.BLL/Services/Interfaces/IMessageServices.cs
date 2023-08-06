using ChatHub.BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatHub.BLL.Services.Interfaces
{
    public interface IMessageServices
    {
        Task<ServiceResult<List<MessageDTO>>> GetMessageOfPrivateChat(string senderusername, string receiverusername);
        Task<ServiceResult<MessageDTO>> CreateMessage(MessageDTO messageDTO);
    }
}

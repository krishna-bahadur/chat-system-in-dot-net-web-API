using ChatHub.BLL.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatHub.BLL.Services.Interfaces
{
    public interface IMessageServices
    {
        Task<ServiceResult<List<MessageDTO>>> GetMessageOfPrivateChat(string? senderusername, string receiverusername);
        Task<ServiceResult<List<MessageDTO>>> GetLastMessageOfPrivateChat(string? senderusername);
        Task<ServiceResult<MessageDTO>> CreateMessage(MessageDTO messageDTO);
        Task<ServiceResult<string>> SaveFile(FileDTO fileDTO);
    }
}

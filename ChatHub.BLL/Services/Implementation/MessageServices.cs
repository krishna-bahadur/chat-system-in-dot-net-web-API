using ChatHub.BLL.DTOs;
using ChatHub.BLL.Services.Interfaces;
using ChatHub.DAL.Datas;
using ChatHub.DAL.Entities;
using ChatHub.DAL.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatHub.BLL.Services.Implementation
{
    public class MessageServices : IMessageServices
    {
        private readonly IRepository<Message> _messageRepository;
        private readonly ChatHubDbContext _dbContext;
        private readonly IEncryptionService _encryptionService;
        private readonly IUploadImages _uploadImages;

        public MessageServices(IRepository<Message> messageRepository, ChatHubDbContext dbContext, IEncryptionService encryptionService, IUploadImages uploadImages)
        {
            _messageRepository = messageRepository;
            _dbContext = dbContext;
            _encryptionService = encryptionService;
            _uploadImages = uploadImages;
        }

        public async Task<ServiceResult<MessageDTO>> CreateMessage(MessageDTO messageDTO)
        {
            using(var context =await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    Message message = new Message()
                    {
                        MessageId = Guid.NewGuid().ToString(),
                        Messages = await _encryptionService.Encrypt(messageDTO?.Messages),
                        SenderUsername = messageDTO.SenderUsername,
                        ReceiverUsername = messageDTO.ReceiverUsername,
                        IsFile = messageDTO.IsFile,
                        DepartmentId = messageDTO.DepartmentId,
                        DateTime = DateTime.Now,
                        IsDeletedBySender = false,
                        IsDeleted = false,
                    };
                    var message1 = await _messageRepository.AddAsync(message);
                    await context.CommitAsync();
                    await context.DisposeAsync();
                    return new ServiceResult<MessageDTO>(true);
                }
                catch(Exception ex)
                {
                    await context.RollbackAsync();
                    await context.DisposeAsync();
                    return new ServiceResult<MessageDTO>(false, errors: new[] { ex.Message });
                }
            }
        }



        public async Task<ServiceResult<List<MessageDTO>>> GetLastMessageOfPrivateChat(string? senderusername)
        {
            //List<MessageDTO> messageDTOs = new List<MessageDTO>();
            //List<Message> messages = new List<Message>();
            //if (!string.IsNullOrEmpty(senderusername))
            //{
            //    messages = await _messageRepository.OrderBy(x => x.ReceiverUsername == senderusername && !x.IsDeleted && !x.IsDeletedBySender, x => x.DateTime);

            //}
            throw new NotImplementedException();
        }


        public async Task<ServiceResult<List<MessageDTO>>> GetMessageOfPrivateChat(string senderusername, string? receiverusername)
        {
            List<MessageDTO> messageDTOs = new List<MessageDTO>();
            List<Message> messages = new List<Message>();
            if(!string.IsNullOrEmpty(senderusername) && !string.IsNullOrEmpty(receiverusername))
            {
                messages = await _messageRepository.OrderBy(x => x.SenderUsername == senderusername && x.ReceiverUsername == receiverusername || x.SenderUsername == receiverusername && x.ReceiverUsername == senderusername && !x.IsDeleted && !x.IsDeletedBySender, x => x.DateTime);
            }
            if (messages.Count > 0)
            {
                foreach (var message in messages)
                {
                    messageDTOs.Add(new MessageDTO()
                    {
                        MessageId = message.MessageId,
                        Messages = await _encryptionService.Decrypt(message?.Messages),
                        SenderUsername = message.SenderUsername,
                        ReceiverUsername = message.ReceiverUsername,
                        IsFile = message.IsFile,
                        DepartmentId = message.DepartmentId,
                        DateTime = message.DateTime,
                        IsDeletedBySender = message.IsDeletedBySender,
                        IsDeleted = message.IsDeleted,
                    });
                }
            }
            return new ServiceResult<List<MessageDTO>>(true, messageDTOs);
        }

        public async Task<ServiceResult<string>> SaveFile(FileDTO fileDTO)
        {
            var FileUrl = "";
            if (fileDTO.File != null)
            {
                FileUrl = await _uploadImages.UploadImageAsync(fileDTO.File);
                return new ServiceResult<string>(true, FileUrl);
            }
            return new ServiceResult<string>(false);
        }
    }
}

using ChatHub.BLL.DTOs;
using ChatHub.BLL.Services.Interfaces;
using ChatHub.DAL.Datas;
using ChatHub.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;

namespace ChatHub.BLL.Services.Implementation
{
    [Authorize]
    public class ChatServices : Hub
    {
        private static readonly List<User> users = new List<User>();
        private static readonly List<Keys> keys = new List<Keys>();
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMessageServices _messageServices;
        private readonly IUploadImages _uploadImages;

        public ChatServices(UserManager<ApplicationUser> userManager, IMessageServices messageServices, IUploadImages uploadImages)
        {
            _userManager = userManager;
            _messageServices = messageServices;
            _uploadImages = uploadImages;
        }


        #region Chat
        public async Task SendMessage(string senderUsername, string receiverUsername, string EncryptedMessage, string message, bool? isFile)
        {
            try
            {
                //var encryptedCipher = JsonConvert.DeserializeObject<EncryptedCipher>(EncryptedMessage);

                var sender = await _userManager.FindByNameAsync(senderUsername);
                var recipientConnectionId = GetConnectionId(receiverUsername);
                var datetime = DateTime.Now;
                await SaveMessage(senderUsername, receiverUsername, message, isFile);
                if (recipientConnectionId != null)
                {
                    await Clients.Client(recipientConnectionId).SendAsync("ReceiveMessage", senderUsername, receiverUsername, EncryptedMessage, "", datetime, isFile);
                }
                var senderConnectionId = GetConnectionId(senderUsername);
                if (senderConnectionId != null)
                {
                    await Clients.Client(senderConnectionId).SendAsync("ReceiveMessage", senderUsername, receiverUsername, EncryptedMessage, "", datetime, isFile);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override Task OnConnectedAsync()
        {
            var username = Context.User.Identity.Name;
            var user = users.SingleOrDefault(x => x.Username == username);
            if (user != null)
            {
                user.ConnectionId = Context.ConnectionId;
            }
            else
            {
                users.Add(new User()
                {
                    Username = username,
                    ConnectionId = Context.ConnectionId
                });
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            users.RemoveAll(x => x.ConnectionId == Context.ConnectionId);
            keys.RemoveAll(x => x.username == Context.User.Identity.Name);
            return base.OnDisconnectedAsync(exception);
        }

        public string GetConnectionId(string username)
        {
            var user = users.SingleOrDefault(x => x.Username == username);
            if (user is not null)
            {
                return user.ConnectionId;
            }
            return null;
        }
        public void SaveKey(string username, string privateKey)
        {
            var k = keys.SingleOrDefault(x => x.username == username);
            if (k != null)
            {
                k.privateKey = privateKey;
            }
            else
            {
                keys.Add(new Keys()
                {
                    username = username,
                    privateKey = privateKey
                });
            }
        }
        public string GetKey(string username)
        {
            var key = keys.SingleOrDefault(x => x.username == username);
            return key?.privateKey;
        }

        private async Task<string> SaveMessage(string senderUsername, string receiverUsername, string message,bool? IsFile)
        {

            MessageDTO messageDTO = new MessageDTO()
            {
                SenderUsername = senderUsername,
                ReceiverUsername = receiverUsername,
                Messages = message,
                IsFile = IsFile,
            };
            var m = await _messageServices.CreateMessage(messageDTO);
            return null;
        }
        private async Task<string> SaveFile(string senderUsername, string receiverUsername, string FileURL)
        {

            MessageDTO messageDTO = new MessageDTO()
            {
                SenderUsername = senderUsername,
                ReceiverUsername = receiverUsername,
                Messages = FileURL,
                IsFile = true,
            };
            var m = await _messageServices.CreateMessage(messageDTO);
            return null;
        }

        public bool IsUserOnline(string username)
        {
            var user = users.SingleOrDefault(x => x.Username == username);
            if (user is not null)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region Video Chat
        public async Task SendOffer(string offer)
        {
            // Send the offer to the specified user
            var id = users.SingleOrDefault(x => x.Username == "suman")?.ConnectionId;
            await Clients.Client(id).SendAsync("ReceiveOffer", offer);
        }

        public async Task SendAnswer(string answer)
        {
            var id = users.SingleOrDefault(x => x.Username == "suman")?.ConnectionId;
            await Clients.Client(id).SendAsync("ReceiveAnswer", answer);
        }

        public async Task StartVideoChat()
        {
            // Notify the connected client to start streaming
            await Clients.All.SendAsync("ReceiveStartVideoChat");
        }

        public async Task SendVideoStream(byte[] streamData)
        {
            // Broadcast the received video stream to all connected clients
            await Clients.All.SendAsync("ReceiveStream", streamData);
        }

        public async Task SendSignal(string caller, string receiver, string signalData)
        {
            // Send the signalData to the receiver using their username
            var user = users.FirstOrDefault(x => x.Username == receiver);
            if (user != null)
            {
                await Clients.Client(user?.ConnectionId).SendAsync("ReceiveSignal", caller, receiver, signalData);
            }
        }
        public async Task CallAcepted(string callerUsername, string receiverUsername)
        {
            // Send the signalData to the receiver using their username
            var user = users.FirstOrDefault(x => x.Username == receiverUsername);
            if (user != null)
            {
                await Clients.Client(user?.ConnectionId).SendAsync("ReceiverAcceptYourCall", callerUsername, receiverUsername);
            }
        }
        public async Task StartCall(string caller, string receiver)
        {
            var user = users.FirstOrDefault(x => x.Username == receiver);
            if (user != null)
            {
                await Clients.Client(user?.ConnectionId).SendAsync("IncomingCall", caller, receiver);
            }
            await Clients.Caller.SendAsync("CallFailed", receiver + " may be not available.");
        }


        #endregion


    }
}

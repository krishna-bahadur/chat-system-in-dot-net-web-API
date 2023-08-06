using ChatHub.BLL.DTOs;
using ChatHub.BLL.Services.Interfaces;
using ChatHub.DAL.Datas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;


namespace ChatHub.BLL.Services.Implementation
{
    [Authorize]
    public class ChatServices : Hub
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private static readonly Dictionary<string, string> userConnectionMap = new Dictionary<string, string>();
        private readonly IMessageServices _messageServices;

        public ChatServices(UserManager<ApplicationUser> userManager, IMessageServices messageServices)
        {
            _userManager = userManager;
            _messageServices = messageServices;
        }

        public async Task SendMessage(string senderUsername, string receiverUsername, string message)
        {
            try
            {
                var sender = await _userManager.FindByNameAsync(senderUsername);
                var recipientConnectionId = GetConnectionId(receiverUsername);
                await SaveMessage(senderUsername, receiverUsername, message);
                if (recipientConnectionId != null)
                {
                    await Clients.Client(recipientConnectionId).SendAsync("ReceiveMessage", senderUsername, receiverUsername, message);
                }
                var senderConnectionId = GetConnectionId(senderUsername);
                if (senderConnectionId != null)
                {
                    await Clients.Client(senderConnectionId).SendAsync("ReceiveMessage", senderUsername, receiverUsername, message);
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
            lock (userConnectionMap)
            {
                if(userConnectionMap.ContainsKey(username))
                {
                    userConnectionMap.Remove(username);
                }
                userConnectionMap.Add(username, Context.ConnectionId);
            }
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            //Remove the disconnected user's entry from the map
            if (userConnectionMap.TryGetValue(Context.User.Identity.Name, out var connectionId))
            {
                userConnectionMap.Remove(Context.User.Identity.Name);
            }
            return base.OnDisconnectedAsync(exception);
        }

        public string GetConnectionId(string username)
        {
            // Look up the connection ID based on the provided username
            if (userConnectionMap.TryGetValue(username, out var connectionId))
            {
                return connectionId;
            }
            return null; // Return null if the username is not found (user 2 is not connected)
        }

        private async Task<string> SaveMessage(string senderUsername, string receiverUsername, string message)
        {
            MessageDTO messageDTO = new MessageDTO()
            {
                SenderUsername = senderUsername,
                ReceiverUsername = receiverUsername,
                Messages = message,
            };
            var m  = await _messageServices.CreateMessage(messageDTO);
            return null;
        }

        public bool IsUserOnline(string username)
        {
            if(!string.IsNullOrEmpty(username))
            {
                return userConnectionMap.ContainsKey(username);
            }
            return false;
        }
    }
}

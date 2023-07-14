using ChatHub.BLL.Services.Interfaces;
using ChatHub.DAL.Datas;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatHub.BLL.Services.Implementation
{
    public class ChatServices : Hub
    {
        //private readonly UserManager<IdentityUser> _userManager;

        //public ChatServices(UserManager<IdentityUser> userManager)
        //{
        //    _userManager = userManager;
        //}

        //public async Task SendMessage(string userId, string message)
        //{
        //    //var user = await _userManager.FindByIdAsync(userId);

        //    //if (user != null)
        //    //{
        //    // Send the message to the specified user
        //    await Clients.User(userId).SendAsync("ReceiveMessage", userId, message);
        //    //await Clients.User(userId).SendAsync("ReceiveMessage", user.UserName, message);
        //    //}
        //}

        private readonly UserManager<ApplicationUser> _userManager;

        public ChatServices(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task SendMessage(string senderId, string receiverId, string message)
        {
            var sender = await _userManager.FindByIdAsync(senderId);
            var receiver = await _userManager.FindByIdAsync(receiverId);

            if (sender != null && receiver != null)
            {
                // Send the message to the specified receiver
                await Clients.User(receiverId).SendAsync("ReceiveMessage", sender.UserName, message);
            }
        }
    }
}

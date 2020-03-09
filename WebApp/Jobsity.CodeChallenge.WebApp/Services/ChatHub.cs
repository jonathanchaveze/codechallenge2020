using Jobsity.CodeChallenge.Bot.Models;
using Jobsity.CodeChallenge.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Jobsity.CodeChallenge.WebApp.Services
{
    public class ChatHub : Hub
    {
        private readonly ChatPersistance _chatPersistance;
        private readonly ChatBot _chatBot;

        public ChatHub(ChatPersistance chatPersistance, ChatBot chatBot)
        {
            _chatPersistance = chatPersistance;
            _chatBot = chatBot;
        }

        private async Task BroadCastMessage(string fullName, string profilePic, DateTime date, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", JsonSerializer.Serialize(new
            {
                fullName,
                profilePic,
                publishedOn = date.ToString("MM/dd/yy hh:mm tt"),
                message
            }));
        }

        [Authorize]
        public async Task UserJoined()
        {
            var currentUser = await _chatPersistance.ChatUsers.FirstAsync(x => x.UserName == Context.User.Identity.Name);
            var response = _chatBot.Read($"/sayhi={currentUser.FullName}");

            await BroadCastMessage(_chatBot.Name, _chatBot.ProfilePic, DateTime.Now, response.ResultText);
        }

        [Authorize]
        public async Task SendMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                await Clients.Caller.SendAsync("Error", "Write the message you want to send.");

                return;
            }

            // Identify message's owner
            var currentUser = await _chatPersistance.ChatUsers.FirstAsync(x => x.UserName == Context.User.Identity.Name);

            // Save the message in database using persistance service (EF)
            var cm = new ChatPost
            {
                ChatUserId = currentUser.Id,
                PublishedOn = DateTime.Now,
                Message = message
            };

            await _chatPersistance.ChatPosts.AddAsync(cm);
            await _chatPersistance.SaveChangesAsync();

            // Broadcast the new message to all people in chat room
            await BroadCastMessage(cm.ChatUser.FullName, cm.ChatUser.ProfilePic, cm.PublishedOn, cm.Message);

            // Let the bot read the message
            var response = _chatBot.Read(message);

            // Broadcast bot response to all users
            if (!string.IsNullOrEmpty(response.ResultText))
            {
                await BroadCastMessage(_chatBot.Name, _chatBot.ProfilePic, DateTime.Now, response.ResultText);
            }
        }
    }
}
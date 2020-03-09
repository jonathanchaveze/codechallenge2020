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

            GetBotInfo();
        }

        private void GetBotInfo()
        {
            var chatBotInfo = _chatPersistance.ChatUsers.Find(_chatBot.ChatUserId);

            if (chatBotInfo == null)
            {
                _chatBot.Name = "Bot without name";
            }
            else
            {
                _chatBot.Name = chatBotInfo.FullName;
                _chatBot.ProfilePic = chatBotInfo.ProfilePic;
            }
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

            var botPost = new ChatPost
            {
                ChatUserId = _chatBot.ChatUserId,
                PublishedOn = DateTime.Now,
                Message = response.ResultText
            };

            // Add post to the unit of work
            await _chatPersistance.ChatPosts.AddAsync(botPost);
            // Persist post message
            await _chatPersistance.SaveChangesAsync();

            // Broadcast welcome message to all users in room
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

            // Save the message unless it's a stock command

            if (!message.ToLower().StartsWith("/stock="))
            {
                // Add post to the unit of work
                await _chatPersistance.ChatPosts.AddAsync(cm);
            }

            // Broadcast the new message to all people in chat room
            await BroadCastMessage(currentUser.FullName, currentUser.ProfilePic, cm.PublishedOn, cm.Message);

            // Let the bot read the message
            var response = _chatBot.Read(message);

            // If the bot has a response for the message
            if (!string.IsNullOrEmpty(response.ResultText))
            {
                // Save bot message in database
                var botPost = new ChatPost
                {
                    ChatUserId = _chatBot.ChatUserId,
                    PublishedOn = DateTime.Now,
                    Message = response.ResultText
                };

                // Add post to the unit of work
                await _chatPersistance.ChatPosts.AddAsync(botPost);                

                // Broadcast to all users in the room
                await BroadCastMessage(_chatBot.Name, _chatBot.ProfilePic, DateTime.Now, response.ResultText);
            }

            // Persist post messages
            await _chatPersistance.SaveChangesAsync();
        }
    }
}
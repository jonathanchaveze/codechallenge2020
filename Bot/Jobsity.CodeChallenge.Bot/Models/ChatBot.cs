using Jobsity.CodeChallenge.Bot.Services;
using System;
using System.Linq;

namespace Jobsity.CodeChallenge.Bot.Models
{
    public class ChatBot
    {        
        private StockService _stockService;

        public string Name { get; private set; }
        public string ProfilePic { get; private set; }

        public ChatBot(string name, string profilePic, string stockServiceEndpoint)
        {
            Name = name;
            ProfilePic = profilePic;
            _stockService = new StockService(stockServiceEndpoint);
        }

        private BotRequest ParseMessage(string message)
        {
            var request = new BotRequest();

            if (message.StartsWith("/"))
            {
                var parts = message.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries)
                                   .ToList();

                var commandText = parts[0].Replace("/", "");
                var parameterText = parts.Count > 1 ? parts[1] : string.Empty;

                if (Enum.TryParse<BotCommand>(commandText, ignoreCase: true, out BotCommand command))
                {
                    request.Command = command;
                }

                request.Parameter = parameterText;
            }

            return request;
        }

        public BotResponse Read(string message)
        {
            var request = ParseMessage(message);
            var response = new BotResponse(request);

            switch (response.Command)
            {
                case BotCommand.SayHi:
                    response.ResultText = $"Hello {request.Parameter}, this is {Name} and welcome to our financial chat room. Feel free to chat with any of our users or type /help if you need my assistance 😁.";
                    break;

                case BotCommand.Help:
                    response.ResultText = "I have the following commands available for you:\n /stock=[code] where [code] is the code name of the stock you want to consult.";
                    break;

                case BotCommand.Stock:
                    try
                    {
                        var stock = _stockService.GetStockByCode(request.Parameter);
                        response.Result = stock;
                        response.ResultText = $"{stock.Code} quote is {stock.Price.ToString("C")} per share.";
                    }
                    catch (Exception e)
                    {
                        response.ResultText = e.Message;
                    }
                    break;
            }

            return response;
        }
    }
}

using Jobsity.CodeChallenge.Bot.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jobsity.CodeChallenge.Testing
{
    [TestClass]
    public class BotTesting
    {
        private readonly string _stockEndpoint = "https://stooq.com/q/l/?f=sd2t2ohlcv&h&e=csv";
        private readonly string _botName = "Misti";
        private ChatBot _bot;

        public BotTesting()
        {
            _bot = new ChatBot(_botName, _stockEndpoint);
        }

        [TestMethod]
        public void SayHi()
        {
            var user = "Jonathan Chavez";
            var response = _bot.Read($"/sayhi={user}");

            Assert.AreEqual(BotCommand.SayHi, response.Command);
            Assert.IsTrue(response.ResultText.StartsWith($"Hello {user},"));
        }

        [TestMethod]
        public void RequestHelp()
        {
            var response = _bot.Read($"/help");

            Assert.AreEqual(BotCommand.Help, response.Command);
            Assert.IsTrue(response.ResultText.StartsWith("I have the following commands available for you:"));
        }

        [TestMethod]
        public void RequestStock()
        {
            var response = _bot.Read($"/stock=aapl.us");

            Assert.AreEqual(BotCommand.Stock, response.Command);
            Assert.IsInstanceOfType(response.Result, typeof(Stock));
        }

        [TestMethod]
        public void RequestUnknownCommand()
        {
            var response = _bot.Read($"/abc");

            Assert.AreEqual(BotCommand.Unknown, response.Command);
        }
    }
}
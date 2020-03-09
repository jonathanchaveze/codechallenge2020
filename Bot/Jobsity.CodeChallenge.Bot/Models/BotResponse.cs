namespace Jobsity.CodeChallenge.Bot.Models
{
    public class BotResponse
    {
        public BotCommand Command { get; set; }
        public string Parameter { get; set; }
        public object Result { get; set; }
        public string ResultText { get; set; }

        public BotResponse(BotRequest request)
        {
            Command = request.Command;
            Parameter = request.Parameter;
        }
    }
}
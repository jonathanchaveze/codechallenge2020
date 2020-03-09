using System;

namespace Jobsity.CodeChallenge.Bot.Models
{
    [Flags]
    public enum BotCommand
    {
        NotCommand = 0,
        Unknown = 1,
        SayHi = 2,
        Help = 4,
        Stock = 8
    }
}
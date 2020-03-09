using System;

namespace Jobsity.CodeChallenge.Bot.Models
{
    [Flags]
    public enum BotCommand
    {
        Unknown = 0,
        SayHi = 1,
        Help = 2,
        Stock = 4
    }
}
using System;

namespace Jobsity.CodeChallenge.WebApp.Models
{
    public class ChatPost
    {
        public Guid Id { get; set; }
        public DateTime PublishedOn { get; set; }
        public string Message { get; set; }
        public string ChatUserId { get; set; }
        public ChatUser ChatUser { get; set; }
    }
}

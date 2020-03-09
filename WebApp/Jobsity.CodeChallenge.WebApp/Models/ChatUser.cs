using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jobsity.CodeChallenge.WebApp.Models
{
    public class ChatUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePic { get; set; }
        public IEnumerable<ChatPost> Posts { get; set; }

        [NotMapped]
        public string FullName { get => $"{FirstName} {LastName}"; }

        public ChatUser()
        {
            Posts = new List<ChatPost>();
        }
    }
}
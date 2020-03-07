using Jobsity.CodeChallenge.WebApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Jobsity.CodeChallenge.WebApp.Services
{
    public class ChatPersistance : IdentityDbContext
    {
        public DbSet<ChatUser> ChatUsers { get; set; }
        public DbSet<ChatPost> ChatPosts { get; set; }

        public ChatPersistance(DbContextOptions options) : base(options)
        {

        }
    }
}
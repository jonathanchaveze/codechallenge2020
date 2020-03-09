using Jobsity.CodeChallenge.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace Jobsity.CodeChallenge.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ChatPersistance _chatPersistance;

        public HomeController(ILogger<HomeController> logger, ChatPersistance chatPersistance)
        {
            _logger = logger;
            _chatPersistance = chatPersistance;
        }

        public async Task<IActionResult> Index()
        {
            // Every time a user joins the chat room, load the last 50 messaged published on.
            var lastMessages = await _chatPersistance.ChatPosts.Include(cm => cm.ChatUser)
                                                               .OrderByDescending(cm => cm.PublishedOn)
                                                               .Take(50)
                                                               .OrderBy(cm => cm.PublishedOn)
                                                               .ToListAsync();

            return View(lastMessages);
        }
    }
}
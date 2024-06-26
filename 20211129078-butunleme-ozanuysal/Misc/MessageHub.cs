using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

public class MessageHub : Hub
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public MessageHub(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task SendMessage(string userId, string content)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            var message = new Message
            {
                Content = content,
                SenderId = userId,
                Timestamp = DateTime.UtcNow
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            await Clients.All.SendAsync("ReceiveMessage", user.UserName, content);
        }
    }
}

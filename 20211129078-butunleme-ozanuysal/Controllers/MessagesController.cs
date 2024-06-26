using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class MessagesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public MessagesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet("GetAllMessages")]
    public async Task<ActionResult<IEnumerable<Message>>> GetMessages()
    {
        //ApplicationUser applicationUser = _userManager.FindByIdAsync(m.SenderId);
        return await _context.Messages.ToListAsync();
    }

    [HttpPost("SendMessage")]
    public async Task<ActionResult<Message>> PostMessage([FromBody] Message message)
    {
        var user = await _userManager.FindByIdAsync(message.SenderId);
        if (user == null)
        {
            return NotFound("User not found");
        }

        message.Timestamp = DateTime.UtcNow;
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMessages), new { id = message.Id }, message);
    }

    [HttpGet("GetMessagesByUserId/{userId}")]
    public async Task<ActionResult<IEnumerable<Message>>> GetMessagesByUserId(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound("User not found");
        }

        var messages = await _context.Messages
            .Where(m => m.SenderId == userId)
            .ToListAsync();

        return Ok(messages);
    }

    [HttpGet("GetMessagesByGroupId/{groupId}")]
    public async Task<ActionResult<IEnumerable<Message>>> GetMessagesByGroupId(string groupId)
    {
        var group = await _context.GroupMemberships.Where(gm => gm.GroupId == groupId).ToListAsync();
        if (group == null)
        {
            return NotFound("Group not found");
        }
        if (group.Count < 1)
        {
            return NotFound("Group not found");
        }

        var messages = await _context.Messages
            .Where(m => m.GroupId == groupId)
            .ToListAsync();

        return Ok(messages);
    }

    [HttpPost("SendMessageToGroup")]
    public async Task<IActionResult> SendMessageToGroup([FromBody] SendMessageToGroupModel model)
    {
        var group = await _context.GroupMemberships.Where(gm => gm.GroupId == model.GroupId).ToListAsync();
        if (group == null)
        {
            return NotFound("Group not found");
        }
        if (group.Count < 1)
        {
            return NotFound("Group not found");
        }

        var sender = await _userManager.FindByIdAsync(model.SenderId);
        if (sender == null)
        {
            return NotFound("Sender not found");
        }

        var groupMembers = await _context.GroupMemberships
            .Where(gm => gm.GroupId == model.GroupId)
            .Select(gm => gm.UserId)
            .ToListAsync();

        foreach (var memberId in groupMembers)
        {
            var message = new Message
            {
                SenderId = model.SenderId,
                RecipientId = memberId,
                GroupId = model.GroupId,
                Content = model.Content,
                Timestamp = DateTime.UtcNow
            };

            _context.Messages.Add(message);
        }

        await _context.SaveChangesAsync();
        return Ok("Message sent to group successfully.");
    }
}

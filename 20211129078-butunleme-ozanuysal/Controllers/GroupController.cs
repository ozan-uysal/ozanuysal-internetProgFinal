using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class GroupController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public GroupController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }


    [HttpPost("AddUsersToGroup")]
    public async Task<IActionResult> AddUsersToGroup(string groupId, string[] userIds)
    {
        foreach (var userId in userIds)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var existingMembership = await _context.GroupMemberships
                .FirstOrDefaultAsync(gm => gm.GroupId == groupId && gm.UserId == userId);
            if (existingMembership != null)
            {
                return BadRequest("User is already a member of this group");
            }

            var groupMembership = new GroupMembership
            {
                GroupId = groupId,
                UserId = userId
            };

            _context.GroupMemberships.Add(groupMembership);
        }

        await _context.SaveChangesAsync();
        return Ok("Users added to group");
    }

    [HttpPost("AddUserToGroup")]
    public async Task<IActionResult> AddUserToGroup(string groupId, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound("User not found");
        }

        var existingMembership = await _context.GroupMemberships
            .FirstOrDefaultAsync(gm => gm.GroupId == groupId && gm.UserId == userId);
        if (existingMembership != null)
        {
            return BadRequest("User is already a member of this group");
        }

        var groupMembership = new GroupMembership
        {
            GroupId = groupId,
            UserId = userId
        };

        _context.GroupMemberships.Add(groupMembership);
        await _context.SaveChangesAsync();

        return Ok("User added to group");
    }

    [HttpGet("GetUsersFromGroup")]
    public async Task<ActionResult<IEnumerable<object>>> GetUsersFromGroup(string groupId)
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

        // Gruba ait kullanıcıları getir
        var userIds = await _context.GroupMemberships
            .Where(gm => gm.GroupId == groupId)
            .Select(gm => gm.UserId)
            .ToListAsync();

        var users = await _context.Users
           .Where(u => userIds.Contains(u.Id))
           .ToListAsync();

        return Ok(users);
    }
}

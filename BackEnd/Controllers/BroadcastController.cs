using BackEnd.Data;
using BackEnd.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BroadcastController(IHubContext<BroadcastHub> hubContext,
        AppDbContext context) : ControllerBase
    {
        [HttpPost("send")]
        public async Task<IActionResult> BrodcastMessage([FromBody] string message)
        {
            var users = await context.UserConnection.AsNoTracking().ToListAsync();
            if (users.Count != 0)
                foreach (var user in users)
                    await hubContext
                        .Clients
                        .Client(user.ConnecitonId)
                        .SendAsync("Subscribe", message);

            return Ok("success");
        }
    }
}

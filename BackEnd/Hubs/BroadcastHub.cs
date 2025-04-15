using System;
using System.Security.Claims;
using BackEnd.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using BackEnd.Model;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Hubs;

[Authorize]
public class BroadcastHub(AppDbContext context) : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context
            .User!
            .Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!
            .Value;

        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException();

        var user = await context 
            .UserConnection 
            .FirstOrDefaultAsync(u => u.UserId == userId);
        if ((user != null))
            context.UserConnection.Remove(user);

        var userConn = new UserConnection
        {
            UserId = userId,
            ConnecitonId = Context.ConnectionId
        };
        context.UserConnection.Add(userConn);
        await context.SaveChangesAsync();
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var user = await context 
                .UserConnection
                .FirstOrDefaultAsync(u => u.ConnecitonId == Context.ConnectionId);

            if(user != null)
            {
                context.UserConnection.Remove(user);
                await context.SaveChangesAsync();
            }
            await base.OnDisconnectedAsync(exception);
    }
}

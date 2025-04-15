using System;
using Microsoft.AspNetCore.SignalR.Client;
using NetcodeHub.Packages.Extensions.LocalStorage;

namespace FrontEnd.Services;

public class BroadcastService(ILocalStorageService localStorageService)
{
    public HubConnection? hubConnection;
    public async Task InitializeAsync(string url)
    {
        string? token = await localStorageService
            .GetEncryptedItemAsStringAsync("token");
        hubConnection = new HubConnectionBuilder()
            .WithUrl(url, option => 
            {
                option.AccessTokenProvider = () => Task.FromResult(token)!;
            }).Build();
        await hubConnection.StartAsync();
    }

    public void SubscribeMessageHandler(Action<string> handler) => 
        hubConnection!.On("Subscribe", handler);
}

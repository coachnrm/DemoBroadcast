using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using FrontEnd;
using FrontEnd.Services;
using NetcodeHub.Packages.Extensions.LocalStorage;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5208") });
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorizationCore();
builder.Services.AddNetcodeHubLocalStorageService();
builder.Services.AddScoped<BroadcastService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthState>();

await builder.Build().RunAsync();

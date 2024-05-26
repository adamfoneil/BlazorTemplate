using Application.Client;
using AuthLibrary;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();
builder.Services.AddRadzenComponents();
builder.Services.AddScoped<BackendWorkIndicator>();

builder.Services.AddHttpClient<CookieHandler>(ApiClient.Name, (sp, client) => builder.HostEnvironment.BaseAddress);


await builder.Build().RunAsync();

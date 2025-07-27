using GamblersHell.Client;
using GamblersHell.Client.Services;
using GamblersHell.Client.StateProviders;
using GamblersHell.Shared;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using System.Net.Http;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Register services
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CookieAuthenticationStateProvider>();
builder.Services.AddAuthorizationCore(); // Add authorization services
builder.Services.AddScoped<CookieAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CookieAuthenticationStateProvider>());

// Register IHttpClientFactory
builder.Services.AddHttpClient(GamblersHellConstants.HttpClientName, client =>
{
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
});

// Register HttpClient directly


// Register MudBlazor services
builder.Services.AddMudServices();

// Register game client services
builder.Services.AddScoped<BlackjackService>();
builder.Services.AddScoped<CardWarsService>();
builder.Services.AddScoped<HereticsRouletteService>();
builder.Services.AddScoped<LadyService>();
builder.Services.AddScoped<PandoraService>();
builder.Services.AddScoped<SlotsService>();
builder.Services.AddScoped<RiddleService>();
builder.Services.AddScoped<PokerService>();
builder.Services.AddScoped<HellPigService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<TransactionService>();
builder.Services.AddSingleton<NavMenuState>();


builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

await builder.Build().RunAsync();

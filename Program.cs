using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ConanTableCraft;
using Microsoft.JSInterop;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<ITitleGeneratorService, TitleGeneratorService>();
builder.Services.AddSingleton<UpdateStatusService>();
await builder.Build().RunAsync();

var host = builder.Build();

// Initialize the TitleGeneratorService
var titleGeneratorService = host.Services.GetRequiredService<ITitleGeneratorService>();
await titleGeneratorService.InitializeAsync();

await host.RunAsync();
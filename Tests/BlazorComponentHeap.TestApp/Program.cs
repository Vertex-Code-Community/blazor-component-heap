using BlazorComponentHeap.Core.Extensions;
using BlazorComponentHeap.Modal.Root;
using BlazorComponentHeap.TestApp;
using BlazorComponentHeap.TestApp.Routing.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.RootComponents.Add<BCHRootModal>("body::after");

var services = builder.Services;

services.AddScoped<FlexibleNavigationManager>();
services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
services.AddBCHComponents();

await builder.Build().RunAsync();
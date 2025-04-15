using BlazorComponentHeap.Components.Modal.Root;
using BlazorComponentHeap.Core.Extensions;
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
// services.AddBCHComponents("_live_key_7d57b5c5-dcbd-49aa-b211-ffc70095b2c4"); // key should be passed here somehow
services.AddBCHComponents("_trial_key_85e39adb-235f-459f-8137-b1648a81093f"); // key should be passed here somehow

await builder.Build().RunAsync();